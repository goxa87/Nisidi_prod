﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventB.DataContext;
using EventB.Models;
using EventB.Services;
using EventB.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventB.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly Context context;

        private readonly ITegSplitter tegSplitter;


        public AccountController(UserManager<User> UM,
            SignInManager<User> SIM,
            ITegSplitter TS,
            Context Context
            )
        {
            userManager = UM;
            signInManager = SIM;
            tegSplitter = TS;
            context = Context;
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
                var loginResult = await signInManager.PasswordSignInAsync(model.LoginProp, model.Password, false, false);

                if (loginResult.Succeeded)
                {
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return RedirectToAction("Start", "Events");
                }                
            }
            
            ModelState.AddModelError("", "Пользователь не найден или неверный пароль");
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
                var user = new User() { Email = model.Email, UserName = model.Email,  Name = model.Name, City = model.City };            

                var createResult = await userManager.CreateAsync(user, model.Password);

                if (createResult.Succeeded)
                {
                    var interests = new List<Interes>();
                    var splitted = tegSplitter.GetEnumerable(model.Tegs);
                    foreach (var inter in splitted)
                    {
                        interests.Add(new Interes { Value = inter });
                    }
                    user.Intereses = interests;
                    context.Users.Update(user);
                    await context.SaveChangesAsync();
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