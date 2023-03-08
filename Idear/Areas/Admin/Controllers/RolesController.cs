
using Idear.Areas.Admin.ViewModels;
using Idear.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Idear.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, QA Manager")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        //ListAllroles
        public async Task<IActionResult> Index()
        {
			var roles = await _roleManager.Roles.ToListAsync();

			var rolesVM = new RolesVM
			{
				IdentityRoles = roles,
				
			};
			return View(rolesVM);
        }

		//Details
		public async Task<IActionResult> Details(string id)
		{
			if (id == null || _roleManager == null)
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
				Id = role.Id,
				RoleName = role.Name,
			};

			return Json(model);
		}

        //CreateRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleName")] RolesVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.RoleName));
                if (result.Succeeded)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }


        //Edit Roles
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("RoleName")] RolesVM model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return NotFound();
                }
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }

        //Delete Roles
 
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            var usersWithRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersWithRole.Any())
            {
                ModelState.AddModelError("", "Cannot delete role as it is chosen by one or more users.");
                return BadRequest(ModelState);
            }
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
