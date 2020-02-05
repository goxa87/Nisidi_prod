using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventB.Auth;
using EventB.DataContext;
using EventB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventB.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AccountController(UserManager<User> UM, SignInManager<User> SIM)
        {
            userManager = UM;
            signInManager = SIM;
        }

        public  IActionResult Login(string returnUrl)
        {
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public async Task<IActionResult> Login(UserLogin model)
        {
            if (ModelState.IsValid)
            {
                var loginResult = await signInManager.PasswordSignInAsync(model.LoginProp, model.Password,false,false);

                if (loginResult.Succeeded)
                {
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return RedirectToAction("Start", "Events");
                }
            }
            ModelState.AddModelError("", "User not found");
            return View(model);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(UserRegistration model)
        {
            if (ModelState.IsValid)
            {
                var person = new Person()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Sity = model.Sity,
                    Interest = model.Tegs,
                    Role = "user"
                };
                var user = new User() { Email = model.Email, UserName = model.Email, Person = person };




                var createResult = await userManager.CreateAsync(user, model.Password);

                if (createResult.Succeeded)
                {                  
                    // надо както его прилепить к пользователю
                    await signInManager.SignInAsync(user, false);
                    return RedirectToAction("Start", "Events");
                }
                else//иначе
                {
                    foreach (var identityError in createResult.Errors)
                    {
                        ModelState.AddModelError("", identityError.Description);
                    }
                }
            }
            return View(model);
        }

        public async Task<ActionResult> Exit()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Start", "Events");
        }

        
    }
}