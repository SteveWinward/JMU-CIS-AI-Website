using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NonProfitSite.Data;
using NonProfitSite.Models;

namespace NonProfitSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var news = await _db.News.OrderByDescending(n => n.PublishedAt).Take(5).ToListAsync();
            var events = await _db.Events.OrderBy(e => e.Start).Take(5).ToListAsync();
            ViewBag.News = news;
            ViewBag.Events = events;
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> Events()
        {
            var items = await _db.Events.OrderByDescending(e => e.Start).ToListAsync();
            return View(items);
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Contact", model);

            var fb = new Feedback
            {
                Name = model.Name,
                Email = model.Email,
                Message = model.Message
            };
            _db.Feedbacks.Add(fb);
            await _db.SaveChangesAsync();

            TempData["Message"] = "Thank you for your feedback. It will be reviewed by our team.";
            return RedirectToAction("Contact");
        }
    }
}
