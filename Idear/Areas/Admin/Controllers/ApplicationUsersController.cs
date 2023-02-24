using Idear.Data;
using Idear.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Idear.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ApplicationUsersController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<ApplicationUser> _userManager;

		public ApplicationUsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_context = context;
		}

		//ListAllUser
		public IActionResult Index()
		{
			return View(_context.ApplicationUsers.ToList());
		}
	}
}
