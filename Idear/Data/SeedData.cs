using Idear.Models;
using Microsoft.EntityFrameworkCore;

namespace Idear.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Categories.Any())
                {
                    return;
                }

                context.Categories.AddRange(
                    new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "General",
                    },
                    new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Workspace",
                    },
                    new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Technical",
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
