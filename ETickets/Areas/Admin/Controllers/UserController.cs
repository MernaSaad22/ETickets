using ETickets.Models;
using ETickets.Repository.IRepository;
using ETickets.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;


namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly UserManager<ApplicationUser> _userManager;


        public UserController(IUserRepository userRepository, IRoleRepository roleRepository, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAsync();

            return View(users.ToList());
        }

        public async Task<IActionResult> ChangeRole(string id)
        {
            var user = _userRepository.GetOne(e => e.Id == id);

            if (user is not null)
            {
                ViewBag.Roles = (await _roleRepository.GetAsync()).ToList().Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Name
                });

                return View(user);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(UserNameWithRoleNameVM userNameWithRoleNameVM)
        {
            if (!ModelState.IsValid)
            {
                return View(userNameWithRoleNameVM);
            }

            var applicationUser = await _userManager.FindByNameAsync(userNameWithRoleNameVM.UserName);

            if (applicationUser is not null)
            {
                var userRoles = await _userManager.GetRolesAsync(applicationUser);
                await _userManager.RemoveFromRolesAsync(applicationUser, userRoles);

                var result = await _userManager.AddToRoleAsync(applicationUser, userNameWithRoleNameVM.RoleName);

                if (result.Succeeded)
                {
                    TempData["Notification"] = "Update User Role Successfully";

                    return RedirectToAction("Index", "User", new { area = "Admin" });
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item.Description);
                    }

                    return View(userNameWithRoleNameVM);
                }
            }

            ModelState.AddModelError("UserName", "Invalid UserName");
            return View(userNameWithRoleNameVM);
        }

        public async Task<IActionResult> LockUnLock(string id)
        {
            var user = _userRepository.GetOne(e => e.Id == id);

            if (user is not null)
            {
                if (user.LockoutEnabled)
                {
                    user.LockoutEnd = DateTime.Now.AddMonths(1);
                    TempData["Notification"] = "Lock User Successfully";
                }
                else
                {
                    user.LockoutEnd = null;
                    TempData["Notification"] = "UnLock User Successfully";
                }

                user.LockoutEnabled = !user.LockoutEnabled;
                await _userManager.UpdateAsync(user);

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }



        // Add these methods to your UserController.cs
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadRoles();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterWithRoleVM model)
        {
            if (!ModelState.IsValid)
            {
                await LoadRoles();
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                Address = model.Address,
                Age = model.Age ,
                Gender = model.Gender
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, model.RoleName);

                if (roleResult.Succeeded)
                {
                    TempData["Success"] = $"User {user.UserName} created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                await _userManager.DeleteAsync(user);
                AddErrors(roleResult.Errors);
            }
            else
            {
                AddErrors(result.Errors);
            }

            await LoadRoles();
            return View(model);
        }

        private async Task LoadRoles()
        {
            var roles = await _roleRepository.GetAsync();
            ViewBag.Roles = new SelectList(roles, "Name", "Name");
        }

        private void AddErrors(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
