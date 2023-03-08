using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Idear.Data;
using Idear.Models;
using Microsoft.AspNetCore.Authorization;
using Idear.Areas.Admin.ViewModels;
using OfficeOpenXml;
using System.IO.Compression;

namespace Idear.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, QA Manager")]
    public class TopicsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public TopicsController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // List Topic
        public async Task<IActionResult> Index()
        {
            var TopicsVM = new TopicsVM()
            {
                Topics = await _context.Topics.ToListAsync()
            };

            return View(TopicsVM);
        }

        // Detail Topic
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Topics == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (topic == null)
            {
                return NotFound();
            }

            return Json(topic);
        }

        // Create Topic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Topic topic)
        {
            if (ModelState.IsValid)
            {
                topic.Id = Guid.NewGuid().ToString();
                _context.Add(topic);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        // Edit Topic
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,ClosureDate,FinalClosureDate")] Topic topic)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    topic.Id = id;
                    _context.Update(topic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TopicExists(topic.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            }
            return BadRequest();
        }

        // Delete Topic
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.Topics == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Topics'  is null.");
            }
            var topic = await _context.Topics.Include(t => t.Ideas).FirstOrDefaultAsync(d => d.Id == id);
            if (topic == null || topic!.Ideas.Any())
            {
                return BadRequest();
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool TopicExists(string id)
        {
            return _context.Topics.Any(e => e.Id == id);
        }

        // export to zip
        public async Task<IActionResult> ExportZip(string id)
        {
            // find topic by id
            var topic = await _context.Topics
                .Include(t => t.Ideas)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (topic == null)
            {
                return NotFound();
            }

            // get ideas of the topic that have file
            var ideas = await _context.Ideas
                .Where(i => i.Topic == topic && !string.IsNullOrEmpty(i.FilePath))
                .ToListAsync();
            if (ideas.Count == 0)
            {
                return BadRequest();
            }

            // generate the zip archive path using top name
            var zipFileName = $"{topic.Name}.zip";
            var zipFilePath = Path.Combine(Path.GetTempPath(), zipFileName);

            // create a zip archive for creating entry later
            using (var zipArchive = new ZipArchive(new FileStream(zipFilePath, FileMode.Create), ZipArchiveMode.Create))
            {
                foreach (var idea in ideas)
                {
                    // create zip entry for each idea
                    var entryName = Path.Combine(Path.GetFileName(idea.Text), Path.GetFileName(idea.FilePath));
                    var zipEntry = zipArchive.CreateEntry(entryName);

                    // create fileStream from idea.FilePath and copy from zip entry to it
                    var fileStream = new FileStream(Path.Combine(_hostingEnvironment.WebRootPath, idea.FilePath), FileMode.Open);
                    using (var zipEntryStream = zipEntry.Open())
                    {
                        await fileStream.CopyToAsync(zipEntryStream);
                    }
                }
            }
            // return the ZIP archive as a file response with the appropriate MIME type and file name
            return File(new FileStream(zipFilePath, FileMode.Open), "application/octet-stream", zipFileName);
        }



        // export to excel
        public async Task<IActionResult> ExportExcel(string id)
        {
            // find topic by id 
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            // get ideas to list
            var ideas = await _context.Ideas
                .Include(i => i.Topic)
                .Where(i => i.Topic == topic)
                .Include(i => i.Views)
                .Include(i => i.Comments)
                .Include(i => i.Reacts)
                .AsSplitQuery()
                .ToListAsync();

            // create a new Excel package
            using (var package = new ExcelPackage())
            {
                // add a worksheet to the package
                var worksheet = package.Workbook.Worksheets.Add("Ideas");

                // add headers to the worksheet
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Text";
                worksheet.Cells[1, 3].Value = "Views";
                worksheet.Cells[1, 4].Value = "Comments";
                worksheet.Cells[1, 5].Value = "Like";
                worksheet.Cells[1, 6].Value = "Dislike";

                // add data to the worksheet
                for (int i = 0; i < ideas.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = ideas[i].Id;
                    worksheet.Cells[i + 2, 2].Value = ideas[i].Text;
                    worksheet.Cells[i + 2, 3].Value = ideas[i].Views!.Sum(v => v.VisitTime);
                    worksheet.Cells[i + 2, 4].Value = ideas[i].Comments.Count;
                    worksheet.Cells[i + 2, 5].Value = ideas[i].Reacts.Where(i => i.ReactFlag == 1).Count();
                    worksheet.Cells[i + 2, 6].Value = ideas[i].Reacts.Where(i => i.ReactFlag == -1).Count();
                }
                // set column widths
                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                // create a memory stream and write the package to it
                var stream = new MemoryStream();
                package.SaveAs(stream);
                // set the position of the stream to the beginning
                stream.Position = 0;
                // return the stream as a file with the topic name as the filename
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{topic.Name}.xlsx");
            }
        }
    }
}
