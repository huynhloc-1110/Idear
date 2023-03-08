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
using MailKit.Security;
using Org.BouncyCastle.Crypto;

namespace Idear.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public CommentsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string cmtText, bool isAnonymous, string ideaId,
            string deadline)
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
            var emailSettings = _configuration.GetSection("EmailSettings");
            var email = emailSettings["Email"];
            var password = emailSettings["Password"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("please do not reply", "plsdonotreply@gmail.com"));
            message.To.Add(MailboxAddress.Parse(user.Email));
            message.Subject = "Someone commented on your idea!";

            var url = Url.Action("Details", "Ideas", new { id = ideaId }, Request.Scheme);

            message.Body = new TextPart("html")
            {
                Text = $"<p>{cmt.User.FullName} has added a <a href=\"{url}#{cmt.Id}\">comment</a> on your \"<b>{cmt.Idea!.Text}</b>\" idea</p>"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                client.Authenticate(email, password);

                client.Send(message);

                client.Disconnect(true);
            }

            return Json(new { id = cmt.Id, user = cmt.User.FullName, dateTime = cmt.Datetime.ToString("d/M/yyyy HH:mm:ss") });
        }
    }
}
