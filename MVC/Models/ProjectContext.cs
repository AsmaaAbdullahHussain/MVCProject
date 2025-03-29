using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mvc.Models;
using mvc.Models.Authorize;

namespace MVC.Models
{
    public class ProjectContext:IdentityDbContext<ApplicationUser>
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options)
        {

        }
        
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<OpeningHour> OpeningHours { get; set; }
        public DbSet<Ad> Ads { get; set; }
    }
}
