using Idear.Areas.Staff.ViewModels;
using Idear.Data;
using Idear.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Logging;

namespace Idear.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IConfiguration config) // add IConfiguration parameter to constructor
        {
            _context = context;
            _userManager = userManager;
            _config = config;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string cmtText, bool isAnonymous, string ideaId,
            string deadline, ILogger<CommentsController> logger)
        {
            // check for comment null or topic deadline meet
            if (!DateTime.TryParse(deadline, out var topicFinalClosureDate))
            {
                return BadRequest("The request is invalid.");
            }
            if (cmtText == "" || cmtText == null || topicFinalClosureDate < DateTime.Now)
            {
                return BadRequest("The request is invalid.");
            }

            try
            {
                var cmt = new Comment()
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = cmtText,
                    IsAnonymous = isAnonymous,
                    Idea = await _context.Ideas.FindAsync(ideaId),
                    User = await _userManager.GetUserAsync(User),
                    Datetime = DateTime.Now
                };

                _context.Comments.Add(cmt);
                await _context.SaveChangesAsync();

                var user = await _userManager.GetUserAsync(User);

                // send email using MailKit

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_config["EmailSettings:SenderEmail"], _config["EmailSettings:SenderName"]));
                message.To.Add(new MailboxAddress(user.Email, "Recipient Name"));
                message.Subject = "Someone commented on your idea!";

                message.Body = new TextPart("plain")
                {
                    Text = $"A new comment has been added: {cmtText}"
                };

                using (var client = new SmtpClient())
                {
                    client.Connect(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:SmtpPort"]), true);

                    client.Authenticate(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);

                    client.Send(message);

                    client.Disconnect(true);
                }

                return Json(new { id = cmt.Id, user = cmt.User.FullName, dateTime = cmt.Datetime.ToString("d/M/yyyy HH:mm:ss") });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending email");
                return StatusCode(500, "An error occurred while sending the email.");
            }
        }
    }
}
