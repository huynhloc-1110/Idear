using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Idear.Data;
using Idear.Models;
using Microsoft.AspNetCore.Authorization;
using Idear.Areas.Admin.ViewModels;
using OfficeOpenXml;
using System.IO.Compression;

namespace Idear.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, QA Manager")]
    public class TopicsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TopicsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // List Topic
        public async Task<IActionResult> Index()
        {
            var TopicsVM = new TopicsVM()
            {
                Topics = await _context.Topics.ToListAsync()
            };

            return View(TopicsVM);
        }

        // Detail Topic
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Topics == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (topic == null)
            {
                return NotFound();
            }

            return Json(topic);
        }

        // Create Topic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Topic topic)
        {
            if (ModelState.IsValid)
            {
                topic.Id = Guid.NewGuid().ToString();
                _context.Add(topic);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        // Edit Topic
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,ClosureDate,FinalClosureDate")] Topic topic)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    topic.Id = id;
                    _context.Update(topic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TopicExists(topic.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            }
            return BadRequest();
        }

        // Delete Topic
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.Topics == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Topics'  is null.");
            }
            var topic = await _context.Topics.Include(t => t.Ideas).FirstOrDefaultAsync(d => d.Id == id);
            if (topic == null || topic!.Ideas.Any())
            {
                return BadRequest("Cannot delete topic as it is had one or more ideas!");
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool TopicExists(string id)
        {
            return _context.Topics.Any(e => e.Id == id);
        }

    }
}
