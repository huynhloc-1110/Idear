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
using System.Drawing.Printing;
using System.Reflection.Metadata.Ecma335;

namespace Idear.Areas.Staff.Controllers
{
	[Authorize]
    [Area("Staff")]
    public class TopicsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public TopicsController(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index(int? page)
		{
            int pageSize = 5;

            return View(PaginatedList<Topic>.Create(await _context.Topics.OrderByDescending(t => t.ClosureDate).
                Include(t => t.Ideas).ToListAsync(), page ?? 1, pageSize));
		}

		public async Task<IActionResult> Details(string id, int? page)
		{
            int pageSize = 5;
            var topic = await _context.Topics
                .Include(t => t.Ideas)!
                    .ThenInclude(i => i.Views)
                .Include(t => t.Ideas)!
                    .ThenInclude(i => i.Comments)
                .Include(t => t.Ideas)!
                    .ThenInclude(i => i.Reacts)
                .FirstOrDefaultAsync(t => t.Id == id);

            ViewBag.TopicId = id;
            ViewBag.Topic = topic;

            return View(PaginatedList<Idea>.Create(await _context.Ideas
                .Where(i => i.Topic == topic)
                .ToListAsync(), page ?? 1, pageSize));
        }
    }
}
