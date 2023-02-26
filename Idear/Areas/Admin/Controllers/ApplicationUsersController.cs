using Idear.Data;
using Idear.Models;
using Idear.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Runtime.Intrinsics.Arm;

namespace Idear.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

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
        public async Task<IActionResult> Index()
        {
            return View(await _context.ApplicationUsers.ToListAsync());
        }

        //UpdateUserRoles
        [HttpGet]
        public async Task<IActionResult> UpdateRoles(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager
                .FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == id)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var userRolesVM = new ApplicationUsersVM
            {
                Roles = await _roleManager.Roles.Select(r => new SelectListItem()
                {
                    Value = r.Id,
                    Text = r.Name,
                    Selected = userRoles.Contains(r.Id)
                }).ToListAsync(),
                AppUser = user
            };

            return View(userRolesVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoles(ApplicationUsersVM model)
        {
            var selectedRoleId = model.Roles
                .Where(x => x.Selected)
                .Select(x => x.Value);
            var AlreadyExistRoleId = await _context.UserRoles
                .Where(x => x.UserId == model.AppUser.Id)
                .Select(x => x.RoleId)
                .ToListAsync();

            var toAdd = selectedRoleId.Except(AlreadyExistRoleId);
            var toRemove = AlreadyExistRoleId.Except(selectedRoleId);

            foreach (var item in toAdd)
            {
                _context.UserRoles.Add(new IdentityUserRole<string>
                {
                    RoleId = item,
                    UserId = model.AppUser.Id
                });
            }
            foreach (var item in toRemove)
            {
                _context.UserRoles.Remove(new IdentityUserRole<string>
                {
                    RoleId = item,
                    UserId = model.AppUser.Id
                });
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        //Edit
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserVM
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserVM model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = model.Email;
            user.FullName = model.FullName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        //Delete 
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserVM
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
            };
            return View(model);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }

    }
}
