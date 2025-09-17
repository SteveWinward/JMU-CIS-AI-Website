using System.Security.Claims;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NonProfitSite.Data;
using NonProfitSite.Models;
using NonProfitSite.Services;

namespace NonProfitSite.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IEmailSender _emailSender;

        public AdminController(AppDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View();
            }

            var user = await _db.AdminUsers.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? user.Email),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var recentFeedback = await _db.Feedbacks.OrderByDescending(f => f.SubmittedAt).Take(10).ToListAsync();
            var totalFeedback = await _db.Feedbacks.CountAsync();
            var pending = await _db.Feedbacks.CountAsync(f => !f.IsArchived);
            ViewBag.TotalFeedback = totalFeedback;
            ViewBag.PendingFeedback = pending;
            return View(recentFeedback);
        }

        [Authorize]
        public async Task<IActionResult> FeedbackList()
        {
            var items = await _db.Feedbacks.OrderByDescending(f => f.SubmittedAt).ToListAsync();
            return View(items);
        }

        [Authorize]
        public async Task<IActionResult> ViewFeedback(int id)
        {
            var item = await _db.Feedbacks.FindAsync(id);
            if (item == null)
                return NotFound();
            return View(item);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveFeedback(int id)
        {
            var item = await _db.Feedbacks.FindAsync(id);
            if (item == null)
                return NotFound();
            item.IsArchived = true;
            await _db.SaveChangesAsync();
            return RedirectToAction("FeedbackList");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ReplyFeedback(int id)
        {
            var item = await _db.Feedbacks.FindAsync(id);
            if (item == null)
                return NotFound();
            return View(item);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplyFeedback(int id, string subject, string body)
        {
            var item = await _db.Feedbacks.FindAsync(id);
            if (item == null)
                return NotFound();
            if (string.IsNullOrWhiteSpace(item.Email))
            {
                TempData["Error"] = "Cannot reply: no email provided by the user.";
                return RedirectToAction("ViewFeedback", new { id });
            }

            await _emailSender.SendEmailAsync(item.Email, subject, body);
            item.IsReplied = true;
            await _db.SaveChangesAsync();
            TempData["Message"] = "Reply sent.";
            return RedirectToAction("ViewFeedback", new { id });
        }

        [Authorize]
        public async Task<IActionResult> NewsList()
        {
            var items = await _db.News.OrderByDescending(n => n.PublishedAt).ToListAsync();
            return View(items);
        }

        [Authorize]
        public async Task<IActionResult> EventsList()
        {
            var items = await _db.Events.OrderByDescending(e => e.Start).ToListAsync();
            return View(items);
        }

        // Simple admin user CRUD (add only for brevity)
        [Authorize]
        [HttpGet]
        public IActionResult CreateAdmin() => View();

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(string email, string name, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View();
            }
            if (await _db.AdminUsers.AnyAsync(u => u.Email == email))
            {
                ModelState.AddModelError(string.Empty, "User with that email already exists.");
                return View();
            }

            var user = new AdminUser
            {
                Email = email,
                Name = name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };
            _db.AdminUsers.Add(user);
            await _db.SaveChangesAsync();
            return RedirectToAction("Dashboard");
        }
    }
}
