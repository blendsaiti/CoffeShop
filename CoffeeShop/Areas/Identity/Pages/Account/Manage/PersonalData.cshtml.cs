using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CoffeeShop.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public PersonalDataModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostDownloadPersonalDataAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var personalData = new Dictionary<string, string>
            {
                { "Email", user.Email ?? "" },
                { "PhoneNumber", user.PhoneNumber ?? "" },
                { "Username", user.UserName ?? "" }
            };

            return new FileContentResult(
                JsonSerializer.SerializeToUtf8Bytes(personalData),
                "application/json")
            {
                FileDownloadName = "PersonalData.json"
            };
        }

        public async Task<IActionResult> OnPostDeletePersonalDataAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                StatusMessage = "Error: Failed to delete personal data.";
                return RedirectToPage();
            }

            return Redirect("~/");
        }
    }
}