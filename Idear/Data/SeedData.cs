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
            CreateTopics();
            CreateDepartments();

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
            CreateUser("K1@gmail.com", "Le K", "K1@gmail.com", "Staff");
            CreateUser("T2@gmail.com", "Tat T ", "T2@gmail.com", "Staff");
            CreateUser("H3@gmail.com", "Nguyen H", "H3@gmail.com", "Staff");
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
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Non-Technical"
                }
            );
            _context.SaveChanges();
        }
        private void CreateTopics()
        {
            _context.Topics.AddRange(
                new Topic
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "New Idea"
                },
                new Topic
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Change/Improve Company"
                }
            );
            _context.SaveChanges();
        }
        private void CreateDepartments()
        {
            _context.Departments.AddRange(
                new Department
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "IT"
                },
                new Department
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Bussiness"
                },
                new Department
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Marketing"
                },
                new Department
                {
                     Id = Guid.NewGuid().ToString(),
                     Name = "Management"
                },
                new Department
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "HR"
                },
                new Department
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Financial"
                },
                new Department
                {
                     Id = Guid.NewGuid().ToString(),
                     Name = "QA"
                }
            );
            _context.SaveChanges();
            var user1 = _context.ApplicationUsers.FirstOrDefault(u => u.UserName == "Loc123@gmail.com")!;
            user1.Department = _context.Departments.FirstOrDefault(d => d.Name == "HR");
            var user2 = _context.ApplicationUsers.FirstOrDefault(u => u.UserName == "K1@gmail.com")!;
            user2.Department = _context.Departments.FirstOrDefault(d => d.Name == "Bussiness");
            var user3 = _context.ApplicationUsers.FirstOrDefault(u => u.UserName == "T2@gmail.com")!;
            user3.Department = _context.Departments.FirstOrDefault(d => d.Name == "Marketing");
            var user4 = _context.ApplicationUsers.FirstOrDefault(u => u.UserName == "H3@gmail.com")!;
            user4.Department = _context.Departments.FirstOrDefault(d => d.Name == "Financial");
            var user5 = _context.ApplicationUsers.FirstOrDefault(u => u.UserName == "Khoa234@gmail.com")!;
            user5.Department = _context.Departments.FirstOrDefault(d => d.Name == "IT");
            var user6 = _context.ApplicationUsers.FirstOrDefault(u => u.UserName == "Huy345@gmail.com")!;
            user6.Department = _context.Departments.FirstOrDefault(d => d.Name == "QA");
            var user7 = _context.ApplicationUsers.FirstOrDefault(u => u.UserName == "Tri456@gmail.com")!;
            user7.Department = _context.Departments.FirstOrDefault(d => d.Name == "QA");
            _context.SaveChanges(); 
        }
    }
}
