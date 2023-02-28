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

        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;
        public DbSet<Department> Departments { get; set; } = default!;
        public DbSet<Idea> Ideas { get; set; } = default!;
        public DbSet<React> Reactes { get; set; } = default!;
        public DbSet<Topic> Topics { get; set; } = default!;
        public DbSet<View> Views { get; set; } = default!;
    }
}