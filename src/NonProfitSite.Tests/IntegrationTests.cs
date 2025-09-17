using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NonProfitSite.Data;
using NonProfitSite.Models;
using Xunit;

namespace NonProfitSite.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add in-memory database for testing
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Build the service provider and create the database
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                    // Seed data
                    SeedData.EnsureSeedDataAsync(db).GetAwaiter().GetResult();
                }
            });
        }
    }

    public class IntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        public IntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task HomePage_ReturnsSuccessAndContainsMission()
        {
            var client = _factory.CreateClient();
            var res = await client.GetAsync("/");
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            Assert.Contains("Our Mission", content);
        }

        [Fact]
        public async Task SubmitFeedback_SavesToDatabase()
        {
            var client = _factory.CreateClient();

            var form = new MultipartFormDataContent
            {
                { new StringContent("Test User"), "Name" },
                { new StringContent("test@example.org"), "Email" },
                { new StringContent("This is an automated test feedback message."), "Message" }
            };

            var res = await client.PostAsync("/Home/SubmitFeedback", form);
            // Expect redirect to Contact
            Assert.True(res.StatusCode == System.Net.HttpStatusCode.Redirect || res.StatusCode == System.Net.HttpStatusCode.RedirectMethod);

            // Verify in DB
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var fb = await db.Feedbacks.OrderByDescending(f => f.SubmittedAt).FirstOrDefaultAsync();
            Assert.NotNull(fb);
            Assert.Equal("Test User", fb.Name);
            Assert.Equal("test@example.org", fb.Email);
            Assert.Equal("This is an automated test feedback message.", fb.Message);
        }
    }
}
