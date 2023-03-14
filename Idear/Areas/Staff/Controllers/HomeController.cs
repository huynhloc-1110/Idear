using Idear.Areas.Staff.ViewModels;
using Idear.Data;
using Idear.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Idear.Areas.Staff.Controllers
{
	[Area("Staff")]
	public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return Challenge();  //redirect to login page
            }

            var homeVM = new HomeVM()
            {
                OpenedTopics = await _context.Topics
                    .Where(t => t.ClosureDate > DateTime.Now)
                    .OrderByDescending(t => t.ClosureDate)
                    .Take(3)
                    .ToListAsync(),
                LatestComments = await _context.Comments
                    .OrderByDescending(c => c.Datetime)
                    .Take(5)
                    .Include(c => c.Idea)
                    .ToListAsync(),
                LatestIdeas = await _context.Ideas
                    .OrderByDescending(i => i.DateTime)
                    .Take(3)
                    .Include(i => i.Topic)
                    .ToListAsync(),
                MostViewedIdeas = await _context.Ideas
                    .Select(idea => new {
                        Idea = idea,
                        ActualViews = idea.Views.Sum(v => v.VisitTime)
                    })
                    .OrderByDescending(idea => idea.ActualViews)
                    .Take(3)
                    .Select(idea => idea.Idea)
                    .Include(i => i.Views)
                    .ToListAsync(),
                MostLikedIdeas = await _context.Ideas
                    .Select(idea => new {
                        Idea = idea,
                        Likes = idea.Reacts.Where(r => r.ReactFlag == 1).Sum(r => r.ReactFlag)
                    })
                    .OrderByDescending(idea => idea.Likes)
                    .Take(3)
                    .Select(idea => idea.Idea)
                    .Include(i => i.Reacts)
                    .ToListAsync()
            };

            return View(homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}