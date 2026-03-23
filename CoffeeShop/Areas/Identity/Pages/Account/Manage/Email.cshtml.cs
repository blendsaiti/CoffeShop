using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Areas.Identity.Pages.Account.Manage
{
    public class EmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public EmailModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string? Email { get; set; }

        public bool EmailConfirmed { get; set; }

        [TempData]
        public string? StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string? NewEmail { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            Email = await _userManager.GetEmailAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var result = await _userManager.SetEmailAsync(user, Input.NewEmail);
                if (!result.Succeeded)
                {
                    StatusMessage = "Error: Failed to change email.";
                    return RedirectToPage();
                }
            }

            StatusMessage = "Your email has been updated.";
            return RedirectToPage();
        }
    }
}