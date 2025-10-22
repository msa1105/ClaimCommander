using Microsoft.EntityFrameworkCore;
using ClaimCommander.Models;

namespace ClaimCommander.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<ClaimCommander.Models.Document> Documents { get; set; }

        // Add this method to configure the model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure precision and scale for decimal properties
            modelBuilder.Entity<Claim>(entity =>
            {
                entity.Property(e => e.ClaimValue).HasPrecision(18, 2);
                entity.Property(e => e.HoursWorked).HasPrecision(18, 2);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.HourlyRate).HasPrecision(18, 2);
            });
        }
    }
}