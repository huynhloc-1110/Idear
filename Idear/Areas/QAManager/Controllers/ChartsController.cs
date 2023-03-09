using Idear.Data;
using Idear.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Idear.Areas.QAManager.Controllers
{
    [Area("QAManager")]
    [Authorize(Roles = "QA Manager")]
    public class ChartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChartsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<Dictionary<string, object>> GetDepartIdeaCount()
        {
            var labels = await _context.Departments
                .Select(d => d.Name)
                .ToListAsync();
            var ideaCounts = await _context.Departments
                .Include(d => d.Users!)
                    .ThenInclude(u => u.Ideas)
                .Select(d => d.Users.SelectMany(u => u.Ideas!).Count())
                .ToListAsync();

            Dictionary<string, object> data = new();
            data["labels"] = labels;
            data["ideaCounts"] = ideaCounts;

            return data;
        }

        public async Task<Dictionary<string, object>> GetDepartUserCount()
        {
            var labels = await _context.Departments
                .Select(d => d.Name)
                .ToListAsync();
            var userCounts = await _context.Departments
                .Include(d => d.Users!)
                    .ThenInclude(u => u.Ideas)
                .Select(d => d.Users.Where(u => u.Ideas!.Count > 0).Count())
                .ToListAsync();

            Dictionary<string, object> data = new();
            data["labels"] = labels;
            data["userCounts"] = userCounts;

            return data;
        }
    }
}
