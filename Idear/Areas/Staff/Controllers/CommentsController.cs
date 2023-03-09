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
using Idear.Data.Services;

namespace Idear.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ISendMailService _sendMailService;

        public CommentsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            ISendMailService sendMailService)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _sendMailService = sendMailService;
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
            var url = Url.Action("Details", "Ideas", new { id = ideaId }, Request.Scheme);

            MailContent content = new MailContent
            {
                To = user.Email,
                Subject = "Someone commented on your idea!",
                Body = $"<p>{cmt.User.FullName} has added a <a href=\"{url}#{cmt.Id}\">comment</a> on your \"<b>{cmt.Idea!.Text}</b>\" idea</p>",
            };

            _ = _sendMailService.SendMail(content);

            return Json(new { id = cmt.Id, user = cmt.User.FullName, dateTime = cmt.Datetime.ToString("d/M/yyyy HH:mm:ss") });
        }
    }
}
