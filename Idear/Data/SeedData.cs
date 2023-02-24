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
            CreateRoles(roles);

            string[] departments = { "QA", "IT", "HR", "Finance" };
            CreateDepartments(departments);

            string[] categories = { "General", "Improvement", "Change/Update" };
            CreateCategories(categories);

            CreateTopics();

            CreateUsers();

        }

        public void Dispose()
        {
            _context.Dispose();
            _roleManager.Dispose();
            _userManager.Dispose();
            GC.SuppressFinalize(this);
        }

        private void CreateRoles(string[] roleList)
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

        private void CreateUsers()
        {
            CreateUser("Loc123@gmail.com", "Le Huynh Loc", "Loc123@gmail.com", "Staff", "QA");
            CreateUser("N1@gmail.com", "Nhan Vien 1", "N1@gmail.com", "Staff", "IT");
            CreateUser("N2@gmail.com", "Nhan Vien 2", "N2@gmail.com", "Staff", "HR");
            CreateUser("N3@gmail.com", "Nhan Vien 3", "N3@gmail.com", "Staff", "Finance");
            CreateUser("Khoa234@gmail.com", "Le Dong Khoa", "Khoa234@gmail.com", "Admin", "QA");
            CreateUser("Huy345@gmail.com", "Nguyen Phuoc Huy", "Huy345@gmail.com", "QA Manager",
                "QA");
            CreateUser("Tri456@gmail.com", "Tat Khai Tri", "Tri456@gmail.com", "QA Coordinator",
                "IT");
        }

        private void CreateUser(string email, string fullName, string password, string role,
            string department)
        {
            var user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                FullName = fullName,
                Department = _context.Departments.First(d => d.Name == department)
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

        private void CreateTopics()
        {
            _context.Topics.AddRange(
                new Topic
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Workshop ideas for IT students",
                    ClosureDate = new DateTime(2023,1,1),
                    FinalClosureDate = new DateTime(2023,2,1)
                },
                new Topic
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Event ideas for welcoming new students",
                    ClosureDate = new DateTime(2023,1,1),
                    FinalClosureDate = new DateTime(2023,12,31)
                },
                new Topic
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Improve work evironment for students and staffs",
                    ClosureDate = new DateTime(2023,6,1),
                    FinalClosureDate = new DateTime(2023,12,31)
                }
            );
            _context.SaveChanges();
        }

        private void CreateDepartments(string[] departments)
        {
            foreach (string department in departments)
            {
                _context.Departments.Add(
                    new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = department
                    }
                );
            }
            _context.SaveChanges();
        }

        private void CreateCategories(string[] categories)
        {
            foreach (string category in categories)
            {
                _context.Categories.Add(
                    new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = category
                    }
                );
            }
            _context.SaveChanges();
        }

    }
}
