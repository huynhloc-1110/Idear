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
		public async Task<IActionResult> Index()
		{
			return View(await _context.Topics.OrderByDescending(t => t.ClosureDate).Include(t => t.Ideas).ToListAsync());
		}
    }
}
