// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace TomorrowsVoices.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [FromQuery] // Add this to capture the forceChange parameter
        public bool ForceChange { get; set; }

        public class InputModel
        {
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; } // Make this optional when ForceChange is true

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            // Check if this is a forced password change
            var claims = await _userManager.GetClaimsAsync(user);
            ForceChange = claims.Any(c => c.Type == "ForcePasswordChange" && c.Value == "true");

            if (ForceChange)
            {
                StatusMessage = "You must change your default password before continuing.";
                Input = new InputModel(); // Initialize without requiring old password
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Check if this is a forced password change
            var claims = await _userManager.GetClaimsAsync(user);
            var isForcedChange = claims.Any(c => c.Type == "ForcePasswordChange" && c.Value == "true");

            IdentityResult changePasswordResult;

            if (isForcedChange)
            {
                // For forced changes, use password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                changePasswordResult = await _userManager.ResetPasswordAsync(user, token, Input.NewPassword);
            }
            else
            {
                // Normal password change requires old password
                if (string.IsNullOrEmpty(Input.OldPassword))
                {
                    ModelState.AddModelError(string.Empty, "Current password is required.");
                    return Page();
                }
                changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            }

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            // Remove the force password change claim if it exists
            if (isForcedChange)
            {
                var forceChangeClaim = claims.FirstOrDefault(c => c.Type == "ForcePasswordChange");
                if (forceChangeClaim != null)
                {
                    await _userManager.RemoveClaimAsync(user, forceChangeClaim);
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToPage();
        }
    }
}