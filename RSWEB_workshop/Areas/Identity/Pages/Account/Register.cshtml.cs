using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using RSWEB_workshop.Areas.Identity.Data;
using RSWEB_workshop.Data;
using RSWEB_workshop.Models;

namespace RSWEB_workshop.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<RSWEB_workshopUser> _signInManager;
        private readonly UserManager<RSWEB_workshopUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RSWEB_workshopContext _context;


        public RegisterModel(
            UserManager<RSWEB_workshopUser> userManager,
            SignInManager<RSWEB_workshopUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RegisterModel> logger,
            RSWEB_workshopContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _context=context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            [Required]
            public string Role { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }
        public async Task<IActionResult> OnPostAsync(string ?returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                if (Input.Role == "Teacher")
                {

                    _context.Add(new Teacher
                    {
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        Degree = "Default",
                        AcademicRank = "Default",
                        OfficeNumber = "0",
                        HireDate = DateTime.Parse("2020-6-7"),
                    });
                    await _context.SaveChangesAsync();

                }
                else if(Input.Role == "Student")
                {
                    _context.Add(new Student
                    {
                        StudentId = 0, //default e vaka
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        AcquiredCredits = 0,
                        CurrentSemester = 0,
                        EducationLevel = "high",
                        EnrollmentDate = DateTime.Parse("2019-09-01")
                    });
                    await _context.SaveChangesAsync();
                }
                var user = new RSWEB_workshopUser { UserName = Input.Email, Email = Input.Email};
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    IdentityResult IR = await EnsureRole(user.Id, Input.Role);
                    if (IR == null)
                    {
                        throw new Exception("Error during role assignment.");
                    }
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
        public async Task<IdentityResult> EnsureRole(string userId, string role)
        {
            IdentityResult IR = null;

            if (!await _roleManager.RoleExistsAsync(role))
            {
                IR = await _roleManager.CreateAsync(new IdentityRole(role));
            }


            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await _userManager.AddToRoleAsync(user, role);

            return IR;
        }
    }

}