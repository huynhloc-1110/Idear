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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Antiforgery;

namespace Idear.Areas.Staff.Controllers
{
    [Authorize]
    [Area("Staff")]
    public class IdeasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAntiforgery _antiforgery;

        public IdeasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IAntiforgery antiforgery)
        {
            _context = context;
            _userManager = userManager;
            _antiforgery = antiforgery;
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
                        .Select(idea => new
                        {
                            Idea = idea,
                            Views = idea.Views.Sum(v=>v.VisitTime)
                        })
                        .OrderByDescending(idea => idea.Views)
                        .Select(idea => idea.Idea);
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

        //Details 
        public async Task<IActionResult> Details(string id)
		{
            if (id == null)
            {
                return NotFound();
            }

            var idea = await _context.Ideas
                .Include(i => i.User)
                .Include(i => i.Topic)
                .Include(i => i.Category)
                .Include(i => i.Comments)!
                    .ThenInclude(c => c.User)
                .Include(i => i.Reacts)
                .Include(i => i.Views)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (idea == null)
            {
                return NotFound();
            }
            var relatedIdeas = await _context.Ideas
                .Where(i => i.Category!.Id == idea.Category!.Id && i.Id != idea.Id)
                .Include(i => i.User)
                .ToListAsync();

            var ideaVM = new IdeasVM
            {
                Idea = idea,
                RelatedIdeas = relatedIdeas,
                Comment = new Comment
                {
                    Idea = idea
                }
            };


            var user = await _userManager.GetUserAsync(User);

            var view = await _context.Views.Where(v=>v.User!.Id == user.Id && v.Idea.Id == idea.Id).FirstOrDefaultAsync();
            if (view != null)
            {
                view.VisitTime++;
                
            }
            else
            {
                _context.Views.Add(
                    new View
                    {

                        Id = Guid.NewGuid().ToString(),
                        VisitTime = 1,
                        Idea = idea,
                        User = user

                    }
                );
            }
            _context.SaveChanges();

            return View(ideaVM);
        }


        // Add react feature
        [HttpGet]
        public IActionResult GetCsrfToken()
        {
            var token = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
            return Json(token);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Like(string ideaId, int reactFlag)
        {
            var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id ==ideaId);
            var user = await _userManager.GetUserAsync(User);
            var react = await _context.Reactes.Where(r => r.User!.Id == user.Id && r.Idea!.Id == ideaId).FirstOrDefaultAsync();
            if (react != null)
            {
                if (react.ReactFlag == reactFlag)
                {
                    react.ReactFlag = 0;
                }
                else
                {
                    react.ReactFlag = reactFlag;
                }
                           
            }
            else
            {
                _context.Reactes.Add(
                    new React
                    {
                        Id = Guid.NewGuid().ToString(),
                        User = user,
                        Idea = idea,
                        ReactFlag = reactFlag,
                    }
                );
            }

            await _context.SaveChangesAsync();

            return ;
        }

    }
}
