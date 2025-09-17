using Microsoft.EntityFrameworkCore;
using NonProfitSite.Models;

namespace NonProfitSite.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<NewsItem> News { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Name).HasMaxLength(200);
                entity.Property(f => f.Email).HasMaxLength(200);
                entity.Property(f => f.Message).IsRequired();
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
            modelBuilder.Entity<NewsItem>(entity =>
            {
                entity.HasKey(n => n.Id);
            });
            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.HasIndex(a => a.Email).IsUnique();
            });
        }
    }
}
