using Admin.AdminDbContext;
using Admin.AdminDbContext.Models;
using Admin.Models.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    public class AccountController : Controller
    {
        List<string> ALL_ROLES = new List<string>() { "admin", "support", "boss" };

        private readonly UserManager<AdminUser> _userManager;
        private readonly SignInManager<AdminUser> _signInManager;
        private readonly AdminContext _adminContext;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AccountController(UserManager<AdminUser> userManager, 
            SignInManager<AdminUser> signInManager,
            AdminContext adminContext,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _adminContext = adminContext;
            _roleManager = roleManager;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                if(!await CheckAnyUserInDb(model))
                {
                    var createResult = await CreateNewUser(model.Login, model.Password);
                    if (createResult.Succeeded)
                    {
                        var logiResult = await _signInManager.PasswordSignInAsync(model.Login, model.Password, true, false);
                        if (logiResult.Succeeded)
                        {
                            var creaatedUser = await _userManager.FindByNameAsync(model.Login);
                            await _userManager.AddToRolesAsync(creaatedUser, ALL_ROLES);
                            if (Url.IsLocalUrl(model.ReturnUrl))
                            {
                                return Redirect(model.ReturnUrl);
                            }
                            return RedirectToAction("Statistic", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Неверные данные для регистрации первого пользователя.");
                        return View(model);
                    }
                }

                var user = await (_userManager.FindByEmailAsync(model.Login));
                if (user == null)
                {
                    ModelState.AddModelError("", "Неверные учетные данные");
                    return View(model);
                }

                var loginResult = await _signInManager.PasswordSignInAsync(model.Login, model.Password, false, false);

                if (loginResult.Succeeded)
                {
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return RedirectToAction("Statistic", "Home");
                }
            }

            ModelState.AddModelError("", "Неверные учетные данные");
            return View(model);
        }


        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize(Roles = "boss")]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "boss")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserVM model)
        {
            if (ModelState.IsValid)
            {
                var createResult = await CreateNewUser(model.Email, model.Password, !string.IsNullOrEmpty(model.AppName) ? model.AppName : null);
                if (createResult.Succeeded)
                {
                    var creaatedUser = await _userManager.FindByNameAsync(model.Email);
                    await _userManager.AddToRoleAsync(creaatedUser, ALL_ROLES[0]);
                    return RedirectToAction("List", "AdminUsers");
                }
                else
                {
                    foreach(var error in createResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        public IActionResult AccessDenied()
        {
            return View("~/Views/Shared/_AccessDenied.cshtml");
        }

        #region PRIVATE

        private async Task<bool> CheckAnyUserInDb(LoginVM model)
        {
            var haveAnyUser = await _adminContext.Users.AnyAsync();
            if (!haveAnyUser)
            {
                await CreateRoles();
            }
            return haveAnyUser;
        }

        private async Task<IdentityResult> CreateNewUser(string login, string password, string appName = "New User")
        {
            var user = new AdminUser()
            {
                UserName = login,
                Email = login,
                UserFio = appName
            };
            return await _userManager.CreateAsync(user, password);
        }

        private async Task CreateRoles()
        {
            foreach(var name in ALL_ROLES)
            {
                if(!await _roleManager.RoleExistsAsync(name))
                {
                    await _roleManager.CreateAsync(new IdentityRole(name));
                }
            }
        }
        #endregion
    }
}
