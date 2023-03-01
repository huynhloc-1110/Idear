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

namespace Idear.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class IdeasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IdeasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Staff/Ideas
        public async Task<IActionResult> Index(string id)
        {
            var topic = await _context.Topics
                .Include(t => t.Ideas)!
                    .ThenInclude(i => i.Views)
                .Include(t => t.Ideas)!
                    .ThenInclude(i => i.Comments)
                .Include(t => t.Ideas)!
                    .ThenInclude(i => i.Reacts)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (topic == null)
            {
                return NotFound();
            }
            var model = new TopicIdeasVM
            {
                Id = topic.Id,
                Name = topic.Name,
                Ideas = topic.Ideas
            };
            return View(model);
        }

    }
}
