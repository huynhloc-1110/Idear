using Idear.Areas.Admin.ViewModels;
using Idear.Areas.Staff.ViewModels;
using Idear.Data;
using Idear.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.IO.Compression;
using System.Reflection.Metadata.Ecma335;

namespace Idear.Areas.Staff.Controllers
{
	[Authorize]
    [Area("Staff")]
    public class TopicsController : Controller
	{
		private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public TopicsController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
		{
			_context = context;
            _hostingEnvironment = hostingEnvironment;
		}
		public async Task<IActionResult> Index(int? page)
		{
            return View(PaginatedList<Topic>.Create(
                await _context.Topics.OrderByDescending(t => t.ClosureDate)
                .Include(t => t.Ideas)
                .ToListAsync(), page ?? 1
            ));
		}

		public async Task<IActionResult> Details(string id, int? page)
		{
            var ideas = await _context.Ideas
                .Include(i => i.Topic)
                .Where(i => i.Topic.Id == id)
                .Include(i => i.Views)
                .Include(i => i.Comments)
                .Include(i => i.Reacts)
                .AsSplitQuery()
                .ToListAsync();
            ViewBag.TopicId = id;
            ViewBag.TopicName = ideas.FirstOrDefault()?.Topic?.Name;
            return View(PaginatedList<Idea>
                .Create(ideas, page ?? 1));
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

            // create a ZIP archive with the name of the topic in the system's temporary folder
            var zipFileName = $"{topic.Name}.zip";
            // get the path to the system's temporary folder, and create the ZIP archive file in that folder instead of the wwwroot
            var zipFilePath = Path.Combine(Path.GetTempPath(), zipFileName);
            using (var zipArchive = new ZipArchive(new FileStream(zipFilePath, FileMode.Create), ZipArchiveMode.Create))
            {
                // for each idea, create a folder with the name of the idea text
                // add the associated file to the folder.
                foreach (var idea in ideas)
                {
                    var folderName = $"{idea.Text}";
                    var fileStream = new FileStream(Path.Combine(_hostingEnvironment.WebRootPath, idea.FilePath), FileMode.Open);
                    var entryName = Path.Combine(Path.GetFileName(folderName), Path.GetFileName(idea.FilePath));
                    var zipEntry = zipArchive.CreateEntry(entryName);
                    using (var zipEntryStream = zipEntry.Open())
                    {
                        // Copy the file to the ZIP archive entry stream
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
