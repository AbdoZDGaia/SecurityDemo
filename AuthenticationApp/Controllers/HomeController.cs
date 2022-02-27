using AuthenticationApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace AuthenticationApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet("login/{provider}")]
        public IActionResult LoginExternal([FromRoute] string provider, [FromQuery] string returnUrl)
        {
            if (User is not null && User.Identities.Any(i => i.IsAuthenticated))
            {
                RedirectToAction(nameof(HomeController.Index), nameof(HomeController));
            }

            returnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
            var authenticationProperties = new AuthenticationProperties { RedirectUri = returnUrl };
            return new ChallengeResult(provider, authenticationProperties);
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate(string username, string password, string returnUrl)
        {
            if (username == "AbdoZ" && password == "P@ss")
            {
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, username));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, "AbdoZ Id"));
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                var items = new Dictionary<string, string>();
                items.Add(".AuthScheme", CookieAuthenticationDefaults.AuthenticationScheme);
                var props = new AuthenticationProperties(items);
                await HttpContext.SignInAsync(claimsPrincipal, props);
                return Redirect(returnUrl);
            }
            ViewData["ReturnUrl"] = returnUrl;
            TempData["Error"] = "Wrong Username or Pass";
            return View("login");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var scheme = User.Claims.FirstOrDefault(c => c.Type == ".AuthScheme").Value;
            if (scheme == "google")
            {
                await HttpContext.SignOutAsync();
                //return Redirect("/");
                return Redirect("https://www.google.com/accounts/Logout?continue=https://appengine.google.com/_ah/logout?continue=https://localhost:7233");
            }
            else
            {
                return new SignOutResult(new[] { CookieAuthenticationDefaults.AuthenticationScheme, scheme });
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Secured()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");
            return View();
        }

        [HttpGet("denied")]
        public IActionResult Denied()
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