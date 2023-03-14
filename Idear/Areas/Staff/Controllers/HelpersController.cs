using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idear.Areas.Staff.Controllers
{
    [Authorize]
    [Area("Staff")]
    public class HelpersController : Controller
    {
        private readonly IAntiforgery _antiforgery;

        public HelpersController (IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public IActionResult GetCsrfToken()
        {
            var token = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
            return Json(token);
        }
    }
}
