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
        public async Task<IActionResult> Create(IdeasVM model)
        {
            var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == model.Idea.Id);
            var currentUser = await _userManager.GetUserAsync(User);

            _context.Comments.Add(
                new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = model.Comment.Text,
                    Datetime = DateTime.Now,
                    Idea = idea,
                    User = currentUser,
                    IsAnonymous = model.Comment.IsAnonymous
                }
            );
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Ideas", new { id = model.Idea.Id});
        }
    }
}
