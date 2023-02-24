using Idear.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Idear.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Idea> Ideas { get; set; }
        public DbSet<React> Reactes { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<View> Views { get; set; }
    }
}