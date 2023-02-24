﻿using Idear.Data;
using Idear.Models;
using Idear.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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



		//UpdateUserRoles
		[HttpGet]
		public async Task<IActionResult> UpdateRoles(string Id)
		{
			ApplicationUsersVM UserRoles = new ApplicationUsersVM();
			var user = _context.ApplicationUsers.Where(x => x.Id == Id).SingleOrDefault();
			var userInRole = _context.UserRoles.Where(x => x.UserId == Id).Select(x => x.RoleId).ToList();
			UserRoles.roles = await _roleManager.Roles.Select(x => new SelectListItem()
			{
				Text = x.Name,
				Value = x.Id,
				Selected = userInRole.Contains(x.Id)
			}).ToListAsync();
			UserRoles.AppUser = user;

			return View(UserRoles);
		}
		[HttpPost]
		public IActionResult UpdateRoles(ApplicationUsersVM model)
		{
			var selectedRoleId = model.roles.Where(x => x.Selected).Select(x => x.Value);
			var AlreadyExistRoleId = _context.UserRoles.Where(x => x.UserId == model.AppUser.Id).Select(x => x.RoleId).ToList();
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
			return RedirectToAction("Index");
		}
	}
}