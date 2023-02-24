using Idear.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Idear.Data
{
    public class SeedData : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public SeedData(IServiceProvider serviceProvider)
        {
            _context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }

        public void Initialize()
        {
            if (_context.ApplicationUsers.Any())
            {
                return;
            }

            string[] roles = { "Staff", "Admin", "QA Manager", "QA Coordinator" };
            CreateSeveralRoles(roles);
            CreateSeveralUser();
            CreateCategories();
        }

        public void Dispose()
        {
            _context.Dispose();
            _roleManager.Dispose();
            _userManager.Dispose();
            GC.SuppressFinalize(this);
        }

        private void CreateSeveralRoles(string[] roleList)
        {
            foreach (string role in roleList)
            {
                if (!_context.Roles.Any(r => r.Name == role))
                {
                    var createdResult = _roleManager.CreateAsync(new IdentityRole(role)).Result;
                    if (!createdResult.Succeeded)
                    {
                        throw new Exception(string.Join(";", createdResult.Errors));
                    }
                }
            }
        }

        private void CreateSeveralUser()
        {
            CreateUser("Loc123@gmail.com", "Le Huynh Loc", "Loc123@gmail.com", "Staff");
            CreateUser("Khoa234@gmail.com", "Le Dong Khoa", "Khoa234@gmail.com", "Admin");
            CreateUser("Huy345@gmail.com", "Nguyen Phuoc Huy", "Huy345@gmail.com", "QA Manager");
            CreateUser("Tri456@gmail.com", "Tat Khai Tri", "Tri456@gmail.com", "QA Coordinator");
        }

        private void CreateUser(string email, string fullName, string password, string role)
        {
            var user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                FullName = fullName
            };

            if (!_context.Users.Any(u => u.UserName == user.UserName)) {
                var createdResult = _userManager.CreateAsync(user, password).Result;
                if (!createdResult.Succeeded)
                {
                    throw new Exception(string.Join(";", createdResult.Errors));
                }
            }

            var assignedResult = _userManager.AddToRoleAsync(user, role).Result;
            if (!assignedResult.Succeeded)
            {
                throw new Exception(string.Join(";", assignedResult.Errors));
            }
        }

        private void CreateCategories()
        {
            _context.Categories.AddRange(
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "General"
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Technical"
                }
            );
            _context.SaveChanges();
        }
    }
}
