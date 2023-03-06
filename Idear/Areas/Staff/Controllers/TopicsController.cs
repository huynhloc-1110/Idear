using Idear.Areas.Admin.ViewModels;
using Idear.Areas.Staff.ViewModels;
using Idear.Data;
using Idear.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
		public async Task<IActionResult> Index(int? pageNumber)
		{
            int pageSize = 5;

            return View(PaginatedList<Topic>.Create(await _context.Topics.OrderByDescending(t => t.ClosureDate).
                Include(t => t.Ideas).ToListAsync(), pageNumber ?? 1, pageSize));
		}

		public async Task<IActionResult> Details(string id)
		{
            var topic = await _context.Topics
                .Include(t => t.Ideas)!
                    .ThenInclude(i => i.Views)
                .Include(t => t.Ideas)!
                    .ThenInclude(i => i.Comments)
                .Include(t => t.Ideas)!
                    .ThenInclude(i => i.Reacts)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (topic == null)
            {
                return NotFound();
            }
            var model = new TopicIdeasVM
            {
                Id = topic.Id!,
                Name = topic.Name!,
                Ideas = topic.Ideas!
            };

            return View(model);
        }
    }
}
