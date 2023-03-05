using Idear.Data;
using Idear.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Idear.Areas.Staff.Controllers
{
    [Authorize]
    [Area("Staff")]
    public class ReactsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReactsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Like(string ideaId, int reactFlag)
        {
            var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == ideaId);
            var user = await _userManager.GetUserAsync(User);
            var react = await _context.Reactes.Where(r => r.User == user && r.Idea == idea).FirstOrDefaultAsync();
            if (react != null)
            {
                if (react.ReactFlag == reactFlag)
                {
                    react.ReactFlag = 0;
                }
                else
                {
                    react.ReactFlag = reactFlag;
                }
            }
            else
            {
                react = new React
                {
                    Id = Guid.NewGuid().ToString(),
                    User = user,
                    Idea = idea,
                    ReactFlag = reactFlag,
                };
                _context.Reactes.Add(react);
            }
            await _context.SaveChangesAsync();

            var likeCount = await _context.Reactes
                .Where(r => r.Idea == idea)
                .CountAsync(r => r.ReactFlag == 1);
            var dislikeCount = await _context.Reactes
                .Where(r => r.Idea == idea)
                .CountAsync(r => r.ReactFlag == -1);

            return Json(new { flag = react.ReactFlag, likeCount, dislikeCount });
        }
    }
}
