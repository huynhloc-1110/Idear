using Idear.Areas.Staff.ViewModels;
using Idear.Data;
using Idear.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Idear.Areas.Staff.Controllers
{
    [Authorize]
    [Area("Staff")]
    public class ReportsController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public ReportsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<IActionResult> Create(string id, string type)
		{
			if (type == null || id == null)
			{
				return BadRequest();
			}
			ReportVM reportVM = new();
			switch (type)
			{
				case "Idea":
					reportVM.ReportedIdeaId = id;
					reportVM.ReportedIdea = await _context.Ideas
						.Include(i => i.User)
						.FirstOrDefaultAsync(i => i.Id == id);
                    break;
				case "Comment":
					reportVM.ReportedCommentId = id;
					reportVM.ReportedComment = await _context.Comments
                        .Include(c => c.User)
						.Include(c => c.Idea)
                        .FirstOrDefaultAsync(c => c.Id == id);
                    break;
				default:
					return BadRequest();
			}

			if (reportVM.ReportedComment == null && reportVM.ReportedIdea == null)
			{
				return BadRequest();
			}
			return View(reportVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create
			([Bind("Reason,ReportedIdeaId,ReportedCommentId")]ReportVM reportVM)
		{
			// load reported idea or comment from the passed id from form submission
            if (!string.IsNullOrEmpty(reportVM.ReportedIdeaId))
            {
                reportVM.ReportedIdea = await _context.Ideas
                    .Include(i => i.User)
                    .FirstOrDefaultAsync(i => i.Id == reportVM.ReportedIdeaId);
            }
            if (!string.IsNullOrEmpty(reportVM.ReportedCommentId))
            {
                reportVM.ReportedComment = await _context.Comments
                    .Include(c => c.User)
                    .Include(c => c.Idea)
                    .FirstOrDefaultAsync(c => c.Id == reportVM.ReportedCommentId);
            }

            if (!ModelState.IsValid)
			{
				return View(reportVM);
            }

			Report report = new()
			{
				Id = Guid.NewGuid().ToString(),
				DateTime = DateTime.Now,
				Reason = reportVM.Reason,
				ReportedIdea = reportVM.ReportedIdea,
				ReportedComment = reportVM.ReportedComment,
				Reporter = await _userManager.GetUserAsync(User)
			};
			_context.Reports.Add(report);
			await _context.SaveChangesAsync();

			string? targetUrlId = "";
			if (reportVM.ReportedIdea != null)
			{
				targetUrlId = reportVM.ReportedIdea.Id;
			}
			if (reportVM.ReportedComment != null)
			{
				targetUrlId = reportVM.ReportedComment.Idea!.Id;
			}

			TempData["SuccessMessage"] = "Your report has been sent successfully.";
            return RedirectToAction("Details", "Ideas", new { id = targetUrlId });
        }
	}
}
