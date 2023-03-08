using Idear.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.IO.Compression;
using System.Net.Mime;

namespace Idear.Areas.QAManager.Controllers
{
    [Area("QAManager")]
    [Authorize(Roles = "QA Manager")]
    public class ExportController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;

        public ExportController(IWebHostEnvironment hostingEnvironment, ApplicationDbContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }
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
                    using var fileStream = new FileStream(Path.Combine(_hostingEnvironment.WebRootPath, idea.FilePath), FileMode.Open);
                    using var zipEntryStream = zipEntry.Open();
                    fileStream.CopyTo(zipEntryStream);
                }
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
            System.IO.File.Delete(zipFilePath);

            // return the ZIP archive as a file response with the appropriate MIME type and file name
            return File(fileBytes, MediaTypeNames.Application.Zip, zipFileName);
        }


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
