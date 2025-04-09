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
        public DbSet<CategoryFeatures>CategoryFeatures { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<OpeningHour> OpeningHours { get; set; }
        public DbSet<Ad> Ads { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Checkout> Checkouts { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        // إضافة للمعاملات في قاعدة البيانات
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // إعداد علاقة العمل التجاري بالباقة (إذا لم تكن موجودة)
            modelBuilder.Entity<Business>()
                .HasOne(b => b.Package)
                .WithMany(p => p.Businesses)
                .HasForeignKey(b => b.PackageId);
        }
    }
}
