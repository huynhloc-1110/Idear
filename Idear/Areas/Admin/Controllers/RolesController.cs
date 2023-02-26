
using Idear.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Idear.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        //ListAllroles
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

		//CreateRoles
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole model)
		{
			//advoid duplicate
			if (!(await _roleManager.RoleExistsAsync(model.Name)))
			{
				await _roleManager.CreateAsync(new IdentityRole(model.Name));
			}
			return RedirectToAction("Index");
		}

		//Edit Roles
		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var role = await _roleManager.FindByIdAsync(id);
			if (role == null)
			{
				return NotFound();
			}

			var model = new RolesVM
			{
				RoleName = role.Name,
				Id = role.Id,
			};
			return View(model);
		}
		[HttpPost]
        [ValidateAntiForgeryToken]
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
		public async Task<IActionResult> Delete(string id)
		{
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var model = new RolesVM
			{
				RoleName = role.Name,
				Id = role.Id
			};
			return View(model);
		}
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
		{
			var role = await _roleManager.FindByIdAsync(id);
			var result = await _roleManager.DeleteAsync(role);
			return RedirectToAction("Index");
		}
	}
}
