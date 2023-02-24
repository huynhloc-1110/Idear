
using Idear.ViewModels;
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
        public IActionResult Index()
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
			return RedirectToAction("Index");
		}



		//Edit Roles
		[HttpGet]
		public async Task<IActionResult> Edit(string Id)
		{
			var role = await _roleManager.FindByIdAsync(Id);
			var model = new RolesVM
			{
				RoleName = role.Name,
				Id = role.Id,
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(RolesVM model)
		{
			var role = await _roleManager.FindByIdAsync(model.Id);
			role.Name = model.RoleName;
			var result = await _roleManager.UpdateAsync(role);
			if (result.Succeeded)
			{
				return RedirectToAction("index");
			}
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}
			return View(model);
		}



		//Delete Roles
		[HttpGet]
		public async Task<IActionResult> Delete(string Id)
		{
			var role = await _roleManager.FindByIdAsync(Id);
			var model = new RolesVM
			{
				RoleName = role.Name,
				Id = role.Id
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Delete(RolesVM model)
		{
			var role = await _roleManager.FindByIdAsync(model.Id);
			role.Name = model.RoleName;
			var result = await _roleManager.DeleteAsync(role);
			if (result.Succeeded)
			{
				return RedirectToAction("Index");
			}
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}
			return View(model);
		}

	}
}
