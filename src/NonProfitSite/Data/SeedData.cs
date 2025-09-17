using System;
using System.Threading.Tasks;
using BCrypt.Net;
using NonProfitSite.Models;
using Microsoft.EntityFrameworkCore;

namespace NonProfitSite.Data
{
    public static class SeedData
    {
        public static async Task EnsureSeedDataAsync(AppDbContext db)
        {
            // If no admin users, create a default admin
            if (!await db.AdminUsers.AnyAsync())
            {
                var admin = new AdminUser
                {
                    Email = "admin@example.org",
                    Name = "Administrator",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin!123")
                };
                db.AdminUsers.Add(admin);
            }

            // Seed a sample news item
            if (!await db.News.AnyAsync())
            {
                db.News.Add(new NewsItem
                {
                    Title = "Welcome to Our Organization",
                    Summary = "This is a seeded news item.",
                    Content = "We are pleased to welcome visitors to our new site.",
                    PublishedAt = DateTime.UtcNow
                });
            }

            // Seed a sample event
            if (!await db.Events.AnyAsync())
            {
                db.Events.Add(new Event
                {
                    Title = "Community Meet & Greet",
                    Description = "Join us for our monthly community meet-up.",
                    Location = "Main Hall",
                    Start = DateTime.UtcNow.AddDays(10),
                    End = DateTime.UtcNow.AddDays(10).AddHours(2)
                });
            }

            await db.SaveChangesAsync();
        }
    }
}
