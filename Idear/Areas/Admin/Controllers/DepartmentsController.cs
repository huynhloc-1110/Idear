using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Idear.Data;
using Idear.Models;
using Idear.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Idear.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, QA Manager")]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public DepartmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/Departments
        public async Task<IActionResult> Index()
        {
            var departmentVM = new DepartmentsVM()
            {
                Departments = await _context.Departments.ToListAsync()
            };

            return View(departmentVM);
        }

        // POST: Admin/Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            if (ModelState.IsValid)
            {
                department.Id=Guid.NewGuid().ToString();
                _context.Add(department);
				await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        // POST: Admin/Departments/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return Json(department);
        }
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name")] Department department)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    department.Id = id;
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            }
            return BadRequest();
        }

		// POST: Admin/Departments/Delete/5
		[HttpDelete]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(string id)
		{
			if (_context.Departments == null)
			{
				return Problem("Entity set 'ApplicationDbContext.Departments'  is null.");
			}

			var department = await _context.Departments.Include(d => d.Users).FirstOrDefaultAsync(d => d.Id == id);

			if (department == null || department!.Users.Any())
			{
				return BadRequest("Cannot delete department as it is chosen by one or more users!");
			}

			_context.Departments.Remove(department);
			await _context.SaveChangesAsync();
			return Ok();
		}

		private bool DepartmentExists(string id)
        {
          return _context.Departments.Any(e => e.Id == id);
        }

    }
}
