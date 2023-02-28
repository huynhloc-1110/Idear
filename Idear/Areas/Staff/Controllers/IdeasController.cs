using Idear.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Idear.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class IdeasController : Controller
	{
		private readonly ApplicationDbContext _context;

		public IdeasController(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index()
		{
			return View(await _context.Topics.ToListAsync());
		}
	}
}
