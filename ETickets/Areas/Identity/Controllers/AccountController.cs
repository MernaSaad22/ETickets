
using ETickets.Models;
using ETickets.Utility;
using ETickets.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace ETickets.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Microsoft.AspNetCore.Identity.SignInManager<ApplicationUser> _signInManager;
        private IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager, IEmailSender emailSender,RoleManager<IdentityRole>roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }


       public async Task<IActionResult> Register()
        {
            //if(_roleManager.Roles.IsNullOrEmpty())
            //{
            //    await _roleManager.CreateAsync(new(SD.SuperAdmin));
            //    await _roleManager.CreateAsync(new(SD.Admin));
            //    await _roleManager.CreateAsync(new(SD.Company));
            //    await _roleManager.CreateAsync(new(SD.Customer));
            //}

                  
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }
            ApplicationUser applicationUser = new()
            {
                Email = registerVM.Email,
                UserName = registerVM.UserName,
                Address = registerVM.Address,
                Age = registerVM.Age,
                Gender = registerVM.Gender

            };
            var result = await _userManager.CreateAsync(applicationUser, registerVM.Password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { area = "Identity", userId = applicationUser.Id, token }, Request.Scheme);

                await _emailSender.SendEmailAsync(registerVM.Email, "Confirmation Your Account", $"Please Confirm Your Account By Clicking <a href='{confirmationLink}'>Here</a>");
                TempData["Notification"] = "Add Account Successfully, Confirm Your Email";
                await _userManager.AddToRoleAsync(applicationUser, SD.Customer);
                return RedirectToAction("MIndex", "Movie", new { area = "Customer" });
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

                return View(registerVM);
            }

        }





        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var applicationUser = await _userManager.FindByEmailAsync(loginVM.EmailOrUserName);

            if (applicationUser is null)
            {
                applicationUser = await _userManager.FindByNameAsync(loginVM.EmailOrUserName);
            }

            if (applicationUser is not null)
            {
                if (applicationUser.LockoutEnabled)
                {
                    var result = await _userManager.CheckPasswordAsync(applicationUser, loginVM.Password);

                    if (result)
                    {
                        // Login

                        await _signInManager.SignInAsync(applicationUser, loginVM.RememberMe);

                        TempData["Notification"] = "Login Successfully";

                        return RedirectToAction("MIndex", "Movie", new { area = "Customer" });
                    }
                    else
                    {
                        ModelState.AddModelError("EmailOrUserName", "Invalid Email Or User Name");
                        ModelState.AddModelError("Password", "Invalid Password");
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"You Have Block Until {applicationUser.LockoutEnd}");
                }
            }
            else
            {
                ModelState.AddModelError("EmailOrUserName", "Invalid Email Or User Name");
                ModelState.AddModelError("Password", "Invalid Password");
            }

            return View(loginVM);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            TempData["Notification"] = "Logout Successfully";

            return RedirectToAction("MIndex", "Movie", new { area = "Customer" });
        }


        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var applicationUser = await _userManager.FindByIdAsync(userId);

            if (applicationUser is not null)
            {

                var result = await _userManager.ConfirmEmailAsync(applicationUser, token);

                if (result.Succeeded)
                {
                    TempData["Notification"] = "Confirm Your Email Successfully";

                    return RedirectToAction("MIndex", "Movie", new { area = "Customer" });
                }
                else
                {
                    TempData["Notification"] = $"{String.Join(", ", result.Errors)}";

                    return RedirectToAction("MIndex", "Movie", new { area = "Customer" });
                }

            }
            else
            {
                return NotFound();
            }
        }


        public IActionResult ResendEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmail(ResendEmailVM resendEmailVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resendEmailVM);
            }

            var applicationUser = await _userManager.FindByEmailAsync(resendEmailVM.EmailOrUserName);

            if (applicationUser is null)
            {
                applicationUser = await _userManager.FindByNameAsync(resendEmailVM.EmailOrUserName);
            }

            if (applicationUser is not null)
            {

                if (!applicationUser.EmailConfirmed)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { area = "Identity", userId = applicationUser.Id, token }, Request.Scheme);

                    await _emailSender.SendEmailAsync(applicationUser!.Email ?? "", "Confirmation Your Account", $"Please Confirm Your Account By Clicking <a href='{confirmationLink}'>Here</a>");

                    TempData["Notification"] = "Send Message Successfully, Confirm Your Email";

                    return RedirectToAction("MIndex", "Movie", new { area = "Customer" });
                }
                else
                {
                    TempData["Notification"] = "Already Confirmed";

                    return RedirectToAction("MIndex", "Movie", new { area = "Customer" });
                }

            }

            ModelState.AddModelError("EmailOrUserName", "Invalid Email Or User Name");
            return View(resendEmailVM);
        }


        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(forgetPasswordVM);
            }

            var applicationUser = await _userManager.FindByEmailAsync(forgetPasswordVM.EmailOrUserName);

            if (applicationUser is null)
            {
                applicationUser = await _userManager.FindByNameAsync(forgetPasswordVM.EmailOrUserName);
            }

            if (applicationUser is not null)
            {

                var token = await _userManager.GeneratePasswordResetTokenAsync(applicationUser);

                var resetPasswordLink = Url.Action("ResetPassword", "Account", new { area = "Identity", userId = applicationUser.Id, token, email = applicationUser.Email }, Request.Scheme);

                await _emailSender.SendEmailAsync(applicationUser!.Email ?? "", "Reset Password", $"Please Reset Your Account Password By Clicking <a href='{resetPasswordLink}'>Here</a>");

                TempData["Notification"] = "Send Message Successfully, Check Your Email";

                return RedirectToAction("MIndex", "Movie", new { area = "Customer" });

            }

            ModelState.AddModelError("EmailOrUserName", "Invalid Email Or User Name");
            return View(forgetPasswordVM);
        }


        public IActionResult ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            return View(resetPasswordVM);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(ResetPassword), resetPasswordVM);
            }

            var applicationUser = await _userManager.FindByEmailAsync(resetPasswordVM.Email);

            if (applicationUser != null && applicationUser.Id == resetPasswordVM.UserId)
            {
                var result = await _userManager.ResetPasswordAsync(applicationUser, resetPasswordVM.Token, resetPasswordVM.Password);

                if (result.Succeeded)
                {
                    await _emailSender.SendEmailAsync(resetPasswordVM.Email, "Reset Password Successfully", $"Reset Password Successfully");

                    TempData["Notification"] = "Reset Password Successfully";

                    return RedirectToAction("MIndex", "Movie", new { area = "Customer" });
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item.Description);
                    }

                    return View(nameof(ResetPassword), resetPasswordVM);
                }
            }

            ModelState.AddModelError("Email", "Invalid Email");
            return View(resetPasswordVM);
        }


        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileVM
            {
                UserName = user.UserName,
                Email = user.Email,
                Address = user.Address,
                Age = user.Age,
                Gender = user.Gender,
                ProfileImage = user.ProfileImage ?? "/images/profiles/default-profile.png"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfileImage(IFormFile newProfileImage)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            if (newProfileImage != null && newProfileImage.Length > 0)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(user.ProfileImage) && user.ProfileImage != "/images/profiles/default-profile.png")
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfileImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save new image
                var uploadsFolder = Path.Combine("wwwroot", "images", "profiles");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(newProfileImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await newProfileImage.CopyToAsync(fileStream);
                }

                user.ProfileImage = uniqueFileName;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("Profile");
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Notification"] = "Password changed successfully";
                return RedirectToAction("Profile");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }

    }
}









