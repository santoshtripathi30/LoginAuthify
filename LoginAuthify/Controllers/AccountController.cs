using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace LoginAuthify.Controllers
{
    public class AccountController : Controller
    {

        [HttpGet("account/login")]  // Explicit route for login page
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpGet("account/login/{provider}")]  // Route with a provider parameter
        public IActionResult ExternalLogin(string provider)
        {
            if (!LoginProviderSettings.IsProviderEnabled(provider))
            {
                return BadRequest("Authentication provider is disabled.");
            }

            var redirectUrl = Url.Action("externallogincallback", "Account", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }


        public async Task<IActionResult> externallogincallback()
        {
            var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authResult.Succeeded)
            {
                return Redirect("Login");
            }

            var claims = authResult.Principal.Identities.FirstOrDefault()?.Claims;
            var emailClaim = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var nameClaim = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var providerClaim = authResult.Properties?.Items[".AuthScheme"];

            if (!string.IsNullOrEmpty(providerClaim))
            {
                // Store user details in memory
                UserStore.AddUser(new UserProfile
                {
                    Name = nameClaim,
                    Email = emailClaim,
                    Provider = providerClaim,
                    LoginTimeUtc = DateTime.UtcNow
                });
            }


            return View("profile", UserStore.GetAllUsers());
        }


        // Logout a specific user by provider
        [HttpPost]
        public async Task<IActionResult> Logout(string provider, string email)
        {
            var user = UserStore.GetAllUsers().FirstOrDefault(u => u.Email == email && u.Provider == provider);
            if (user != null)
            {
                // Remove user from session storage
                UserStore.RemoveUser(email, provider);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Profile");
        }



        public IActionResult ExternalLogin()
        {
            return View();
        }


        [HttpGet("account/profile")]
        public IActionResult Profile()
        {
            var users = UserStore.GetAllUsers();
            return View(users);
        }
    }
}
