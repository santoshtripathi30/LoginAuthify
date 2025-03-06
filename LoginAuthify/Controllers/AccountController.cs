using LoginAuthify.Common;
using LoginAuthify.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace LoginAuthify.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        public AccountController(IConfiguration config)
        {
            _config = config;
        }



        [HttpGet("LoginWithThirdParty")]
        public IActionResult LoginWithThirdParty()
        {
            return View("LoginWithThirdParty");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View("Login");
        }





        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginVM model)
        {
            // Replace this with a real authentication service (e.g., Identity, EF Core)
            if (model.EmailAddress.Equals("santosh", StringComparison.OrdinalIgnoreCase) && model.Password == "pass123")
            {
                var token = JwtTokenGenerator.GenerateToken(model.EmailAddress);

                if (!string.IsNullOrEmpty(token))
                {
                    UserStore.AddUser(new UserProfile
                    {
                        Name = model.EmailAddress,
                        Email = model.EmailAddress,
                        Provider = "JWT",
                        LoginTimeUtc = DateTime.UtcNow
                    });
                }

                return Ok(new { token });
            }

            return Unauthorized("Invalid credentials");
        }


        [HttpGet("login/{provider}")]  // Route with a provider parameter
        public IActionResult ExternalLogin(string provider)
        {
            if (!LoginProviderSettings.IsProviderEnabled(provider))
            {
                return BadRequest("Authentication provider is disabled.");
            }

            var redirectUrl = Url.Action("Externallogincallback", "Account", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }


        public async Task<IActionResult> Externallogincallback()
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

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var users = UserStore.GetAllUsers();
            return View(users);
        }
    }
}
