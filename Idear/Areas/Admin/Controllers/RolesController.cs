using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Idear.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        //ListAllroles
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

		//CreateRoles
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(IdentityRole model)
		{
			//advoid duplicate
			if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
			}
			return RedirectToAction("ListRoles");
		}
	}
}
