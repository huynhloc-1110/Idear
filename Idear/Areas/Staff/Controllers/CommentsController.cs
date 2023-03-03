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
        public async Task<IActionResult> Create([Bind("Text,IsAnonymous,Idea")]Comment comment)
        {
            comment.Idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == comment.Idea!.Id);

            ModelState.Clear();
            TryValidateModel(comment);

            if (!ModelState.IsValid)
            {
                return PartialView("_CreateComment", comment);
            }

            comment.Id = Guid.NewGuid().ToString();
            comment.User = await _userManager.GetUserAsync(User);
            comment.Datetime = DateTime.Now;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return Json("New comment added!");
        }
    }
}
