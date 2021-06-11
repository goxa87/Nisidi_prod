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
        private readonly UserManager<AdminUser> _userManager;
        private readonly SignInManager<AdminUser> _signInManager;
        private readonly AdminContext _adminContext;

        public AccountController(UserManager<AdminUser> userManager, 
            SignInManager<AdminUser> signInManager,
            AdminContext adminContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _adminContext = adminContext;
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
                if(!(await CheckAnyUserInDb(model)))
                {
                    var createResult = await CreateNewUser(model.Login, model.Password);
                    if (createResult.Succeeded)
                    {
                        var logiResult = await _signInManager.PasswordSignInAsync(model.Login, model.Password, false, false);
                        if (logiResult.Succeeded)
                        {
                            if (Url.IsLocalUrl(model.ReturnUrl))
                            {
                                return Redirect(model.ReturnUrl);
                            }
                            return RedirectToAction("Index", "Home");
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
                    return RedirectToAction("Index", "Home");
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


        #region PRIVATE

        private async Task<bool> CheckAnyUserInDb(LoginVM model)
        {
            return await _adminContext.Users.AnyAsync();
        }

        private async Task<IdentityResult> CreateNewUser(string login, string password)
        {

            var user = new AdminUser()
            {
                UserName = login,
                Email = login
            };
            return await _userManager.CreateAsync(user, password);
        }



        #endregion
    }
}
