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
            return View(PaginatedList<Idea>
                .Create(ideas, page ?? 1));
        }
    }
}
