using Idear.Areas.Staff.ViewModels;
using Idear.Data;
using Idear.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Idear.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string cmtText, bool isAnonymous, string ideaId,
            string deadline)
        {
            // check for comment null or topic deadline meet
            if (!DateTime.TryParse(deadline, out var topicFinalClosureDate))
            {
                return BadRequest("The request is invalid.");
            }
            if (cmtText == "" || cmtText == null || topicFinalClosureDate < DateTime.Now )
            {
                return BadRequest("The request is invalid.");
            }

            var cmt = new Comment()
            {
                Id = Guid.NewGuid().ToString(),
                Text = cmtText,
                IsAnonymous = isAnonymous,
                Idea = await _context.Ideas.FindAsync(ideaId),
                User = await _userManager.GetUserAsync(User),
                Datetime = DateTime.Now
            };

            _context.Comments.Add(cmt);
            await _context.SaveChangesAsync();

            return Json(new { id = cmt.Id, user = cmt.User.FullName, dateTime = cmt.Datetime.ToString("d/M/yyyy HH:mm:ss") });
        }
    }
}
