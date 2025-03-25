using Microsoft.EntityFrameworkCore;
using mvc.Models;

namespace MVC.Models
{
    public class ProjectContext:DbContext
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
