using Idear.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Idear.Areas.QAManager.Controllers
{
    [Area("QAManager")]
    [Authorize(Roles = "QA Manager")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Mark(string id, int status)
        {
            var report = await _context.Reports
                .FindAsync(id);
            if (report == null)
            {
                return BadRequest("Report not found");
            }

            report.Status = (Models.ReportStatus)status;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleHide(string id)
        {
            var report = await _context.Reports
                .Include(r => r.ReportedIdea)
                .Include(r => r.ReportedComment)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (report == null)
            {
                return BadRequest("Report not found");
            }

            // toggle is hidden
            if (report.ReportedIdea != null)
            {
                report.ReportedIdea.IsHidden = !report.ReportedIdea.IsHidden;
            }
            if (report.ReportedComment != null)
            {
                report.ReportedComment.IsHidden = !report.ReportedComment.IsHidden;
            }
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
