using LoginAuthify.Models;

using Microsoft.AspNetCore.Mvc;

namespace LoginAuthify.Controllers
{
    public class LoginProviderController : Controller
    {
        private static readonly List<string> AllProviders = new() { "Google", "GitHub", "Facebook", "Twitter", "LinkedIn" };

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ProviderSettingsVM
            {
                AvailableProviders = LoginProviderSettings.AvailableProviders.ToList(),
                EnabledProviders = LoginProviderSettings.GetEnabledProviders().ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Update(List<string> enabledProviders)
        {
            LoginProviderSettings.UpdateEnabledProviders(enabledProviders);
            TempData["SuccessMessage"] = "Authentication providers updated successfully!";
            return RedirectToAction("Index");
        }

    }
}
