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
using System.Security.Claims;
using System.Data;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Logging;
using MailKit.Security;
using Microsoft.Extensions.Configuration;

namespace Idear.Areas.Staff.Controllers
{
    [Authorize]
    [Area("Staff")]
    public class IdeasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public IdeasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        // GET: Staff/Ideas
        public async Task<IActionResult> Index(string orderBy, int? page)
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
                .AsSplitQuery()
                .ToListAsync();
            return View(PaginatedList<Idea>.Create(ideas, page ?? 1));
      
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
                .AsSplitQuery()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (idea == null)
            {
                return NotFound();
            }
            var relatedIdeas = await _context.Ideas
                .Where(i => i.Category == idea.Category && i.Id != idea.Id)
                .Include(i => i.User)
                .ToListAsync();

            var user = await _userManager.GetUserAsync(User);
            var currentUserReact = await _context.Reactes.Where(r => r.User == user && r.Idea == idea).FirstOrDefaultAsync();

            var ideaVM = new IdeasVM
            {
                Idea = idea,
                RelatedIdeas = relatedIdeas,
                Comment = new Comment
                {
                    Idea = idea
                },
                ReactFlag = (currentUserReact != null) ? currentUserReact.ReactFlag : 0
            };

            // Add view when user load the idea detail page
            var view = await _context.Views.Where(v=>v.User == user && v.Idea == idea).FirstOrDefaultAsync();
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

		// Create idea
		[HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CreateIdeasVM
            {
                Topics = await _context.Topics.Where(t => t.ClosureDate >= DateTime.Now).Select(t => new SelectListItem
                {
                    Value = t.Id,
                    Text = t.Name
                }).ToListAsync(),
                Categories = await _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.Id,
                    Text = c.Name
                }).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Text,CategoryId,TopicId,IsAnonymous,AgreeTerms")] CreateIdeasVM model, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                model.Topics = _context.Topics.Select(t => new SelectListItem
                {
                    Value = t.Id,
                    Text = t.Name
                }).ToList();
                model.Categories = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.Id,
                    Text = c.Name
                }).ToList();
                return View(model);
            }

            var topic = await _context.Topics.FirstOrDefaultAsync(t => t.Id == model.TopicId);
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == model.CategoryId);
            var currentUser = await _userManager.GetUserAsync(User);
            if (file != null)
            {
                //string variable uploadDir that represents the folder path where the uploaded file will be stored
                //using the Path.Combine method
                string uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "files");

                //generate a random file name
                var extension = Path.GetExtension(file.FileName);
                var randomFileName = Path.ChangeExtension(Guid.NewGuid().ToString(), extension);

                string filePath = Path.Combine(uploadDir, randomFileName);

                //creates a new file on the file system at the location specified by the filePath variable
                //using the FileStream class and the file.CopyTo method
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                //sets the model.FilePath property to the relative path of the uploaded file
                model.FilePath = @"files\" + randomFileName;
            }

            var idea = new Idea
            {
                Id = Guid.NewGuid().ToString(),
                Text = model.Text,
                DateTime = DateTime.Now,
                Topic = topic,
                Category = category,
                User = currentUser,
                IsAnonymous = model.IsAnonymous,
                FilePath = model.FilePath
            };

            _context.Ideas.Add(idea);
            await _context.SaveChangesAsync();


            //Sending Email to all users who have a QA Coordinator role
            var usersWithRole = await _userManager.GetUsersInRoleAsync("QA Coordinator");
            foreach (var user in usersWithRole)
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var email = emailSettings["Email"];
                var password = emailSettings["Password"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("please do not reply", "plsdonotreply@gmail.com"));
                message.To.Add(MailboxAddress.Parse(user.Email));
                message.Subject = "Someone has created a new idea!";

                var url = Url.Action("Details", "Ideas", new { id = idea.Id }, Request.Scheme);

                message.Body = new TextPart("html")
                {
                    Text = $"<p>{idea.User.FullName} has created a new idea called: <a href=\"{url}\">{idea.Text}</a></p>"
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                    client.Authenticate(email, password);

                    client.Send(message);

                    client.Disconnect(true);
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult DownloadFile(string id)
        {
            var idea = _context.Ideas.FirstOrDefault(i => i.Id == id);
            if (idea == null || string.IsNullOrEmpty(idea.FilePath))
            {
                return NotFound();
            }
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, idea.FilePath);
            var fileStream = new FileStream(filePath, FileMode.Open);
            return File(fileStream, "application/octet-stream", Path.GetFileName(filePath));
        }

        //ListIdeaByUser
        public async Task<IActionResult> ListIdeaByUser( int ?page)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var idea = await _context.Ideas.Where(i => i.User == currentUser).ToListAsync();
            return View(PaginatedList<Idea>.Create(idea, page ?? 1));

        }

        // Delete idea
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Ideas == null)
            {
                return NotFound();
            }

            var idea = await _context.Ideas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (idea == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(User);
            if (idea.User != currentUser) 
			{
				return RedirectToAction("Error", "Home");
            }

            return View(idea);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var idea = await _context.Ideas
                .Include(i => i.Views)
                .Include(i=> i.Comments)
                .Include(i=>i.Reacts)
                .Include(i=>i.User)
				.FirstOrDefaultAsync(i => i.Id == id);

            var currentUser = await _userManager.GetUserAsync(User);
            if (idea.Comments.Any() || idea.User != currentUser) 
			{
				return RedirectToAction("Error", "Home");
            }
            _context.Views.RemoveRange(idea.Views);
            _context.Reactes.RemoveRange(idea.Reacts);          
            _context.Ideas.Remove(idea);
            await _context.SaveChangesAsync();

            // Delte file in server
            if (!string.IsNullOrEmpty(idea.FilePath))
            {
                string oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, idea.FilePath);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            return RedirectToAction("ListIdeaByUser");
        }


		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			var idea = await _context.Ideas
				.Include(i => i.Topic)
				.Include(i => i.Category)
				.FirstOrDefaultAsync(i => i.Id == id);

			if (idea == null)
			{
				return NotFound();
			}

			var model = new CreateIdeasVM
			{
				Id = idea.Id,
				Text = idea.Text,
				TopicId = idea.Topic.Id,
				CategoryId = idea.Category.Id,
				Topics = await _context.Topics.Where(t => t.ClosureDate >= DateTime.Now).Select(t => new SelectListItem
				{
					Value = t.Id,
					Text = t.Name,
					Selected = t.Id == idea.Topic.Id
				}).ToListAsync(),
				Categories = await _context.Categories.Select(c => new SelectListItem
				{
					Value = c.Id,
					Text = c.Name,
					Selected = c.Id == idea.Category.Id
				}).ToListAsync()
			};
            var currentUser = await _userManager.GetUserAsync(User);
            if (idea.User != currentUser)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit([Bind("Id,Text,CategoryId,TopicId,IsAnonymous,AgreeTerms")] CreateIdeasVM model, IFormFile? file)
		{
			if (!ModelState.IsValid)
			{
				model.Topics = await _context.Topics.Where(t => t.ClosureDate >= DateTime.Now).Select(t => new SelectListItem
				{
					Value = t.Id,
					Text = t.Name
				}).ToListAsync();
				model.Categories = await _context.Categories.Select(c => new SelectListItem
				{
					Value = c.Id,
					Text = c.Name
				}).ToListAsync();
				return View(model);
			}

			var idea = await _context.Ideas
				.Include(i => i.Topic)
				.Include(i => i.Category)
				.FirstOrDefaultAsync(i => i.Id == model.Id);

            var currentUser = await _userManager.GetUserAsync(User);
            if (idea.User != currentUser)
            {
                return RedirectToAction("Error", "Home");
            }
            if (idea == null)
			{
				return NotFound();
			}

			var topic = await _context.Topics.FirstOrDefaultAsync(t => t.Id == model.TopicId);
			var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == model.CategoryId);

			idea.Text = model.Text;
			idea.Topic = topic;
			idea.Category = category;
			idea.IsAnonymous = model.IsAnonymous;

			if (file != null)
			{
                if (!string.IsNullOrEmpty(idea.FilePath))
                {
                    string oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, idea.FilePath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                string uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "files");
				var extension = Path.GetExtension(file.FileName);
				var randomFileName = Path.ChangeExtension(Guid.NewGuid().ToString(), extension);
				string filePath = Path.Combine(uploadDir, randomFileName);

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					file.CopyTo(fileStream);
				}

				idea.FilePath = @"files\" + randomFileName;
			}
            _context.Ideas.Update(idea);
			await _context.SaveChangesAsync();

			return RedirectToAction("ListIdeaByUser");
		}
	}
}
