using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Idear.Data;
using Idear.Models;
using Idear.Areas.Staff.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Idear.Areas.Staff.Controllers
{
    [Authorize]
    [Area("Staff")]
    public class IdeasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IdeasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Staff/Ideas
        public async Task<IActionResult> Index(string orderBy)
        {
            IQueryable<Idea> ideaQuery = null;
            switch (orderBy)
            {
                case "like":
                    ideaQuery = _context.Ideas
                        .Select(idea => new
                        {
                            Idea = idea,
                            Likes = idea.Reacts.Where(r => r.ReactFlag == 1).Sum(r => r.ReactFlag)
                        })
                        .OrderByDescending(idea => idea.Likes)
                        .Select(idea => idea.Idea);
                    break;
                case "view":
                    ideaQuery = _context.Ideas
                        .OrderByDescending(i => i.Views!.Count);
                    break;
                case "comment":
                    ideaQuery = _context.Ideas
                        .OrderByDescending(i => i.Comments!.Count);
                    break;
                default:
                    ideaQuery = _context.Ideas
                        .OrderByDescending(i => i.DateTime);
                    break;
            }
            var ideas = await ideaQuery
                .Include(i => i.Views)
                .Include(i => i.Comments)
                .Include(i => i.Reacts)
                .ToListAsync();

            return View(ideas);
        }

    }
}
