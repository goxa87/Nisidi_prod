﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBLib.DataContext;
using EventBLib.Models;
using EventB.Services;
using EventB.ViewModels;
using EventB.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using EventB.Services.SenderServices;
using System.Diagnostics;
using EventBLib.Models.MarketingModels;
using EventB.Services.Logger;
using EventB.Services.ImageService;
using System.Security.Claims;

namespace EventB.Controllers
{
    public class AccountController : Controller
    {
        private const string DEFAULT_IMG_PATH = "/images/defaultimg.jpg";
        private const string IMAGE_SUFFIX = ".jpeg";

        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly Context context;
        readonly IWebHostEnvironment environment;
        private readonly IImageService imageService;
        private readonly ITegSplitter tegSplitter;
        private readonly ILogger logger;

        public AccountController(UserManager<User> UM,
            SignInManager<User> SIM,
            ITegSplitter TS,
            Context Context,
            IWebHostEnvironment _environment,
            ILogger _logger,
            IImageService _imageService
            )
        {
            userManager = UM;
            signInManager = SIM;
            tegSplitter = TS;
            context = Context;
            environment = _environment;
            logger = _logger;
            imageService = _imageService;
        }

        public  IActionResult Login(string returnUrl)
        {
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        [Route("Account/Login")]
        public async Task<IActionResult> Login(UserLogin model)
        {
            if (ModelState.IsValid)
            {
                var user = await (userManager.FindByEmailAsync(model.LoginProp));
                if(user == null)
                {
                    ModelState.AddModelError("", "Пользователь не найден или неверный пароль");
                    return View(model);
                }

                if (!await userManager.IsEmailConfirmedAsync(user))
                    return View("ConfirmEmail", model.LoginProp);
                //await userManager.AddClaimAsync(user, new Claim("user_id", user.Id));
                var loginResult = await signInManager.PasswordSignInAsync(user, model.Password, true, false);

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

        [ValidateAntiForgeryToken, HttpPost]
        [Route("Account/Register")]
        public async Task<IActionResult> Register(UserRegistration model)
        {
            try
            {
                if (!model.AgreePersonalData)
                {
                    ModelState.AddModelError("AgreePersonalData", "Необходимо согласится на обработку персональных данных");
                }
                if (ModelState.IsValid)
                {
                    var user = new User()
                    {
                        Email = model.Email,
                        UserName = model.Email,
                        Name = model.Name,
                        City = model.City,
                        NormalizedName = model.Name.ToUpper(),
                        NormalizedCity = model.City.ToUpper(),
                        Description = model.Description,
                        PhoneNumber = model.PhoneNumber
                    };

                    var fileName = Guid.NewGuid().ToString();
                    string imgSourse = String.Concat("/images/Profileimages/", fileName);
                    string imgMedium = String.Concat("/images/Profileimages/Medium/", "M" + fileName);
                    string imgMini = String.Concat("/images/Profileimages/Mini/", "m" + fileName);

                    if (model.Photo != null)
                    {
                        try
                        {
                            var newImagesDict = new Dictionary<int, string>();

                            newImagesDict.Add(400, imgSourse);
                            newImagesDict.Add(360, imgMedium);
                            newImagesDict.Add(100, imgMini);

                            newImagesDict = await imageService.SaveOriginAndResizedImagesByInputedSizes(model.Photo, IMAGE_SUFFIX, newImagesDict, null);

                            imgSourse = newImagesDict[400];
                            imgMedium = newImagesDict[360];
                            imgMini = newImagesDict[100];
                        }
                        catch (Exception ex)
                        {
                            await logger.LogStringToFile($"Ошибка создания картинок для события : {ex.Message}");
                        }
                    }
                    else
                    {
                        await imageService.SaveResizedImage(environment.WebRootPath + DEFAULT_IMG_PATH, environment.WebRootPath + imgSourse, 500, IMAGE_SUFFIX);
                        imgSourse += IMAGE_SUFFIX;
                        await imageService.SaveResizedImage(environment.WebRootPath + DEFAULT_IMG_PATH, environment.WebRootPath + imgMedium, 360, IMAGE_SUFFIX);
                        imgMedium += IMAGE_SUFFIX;
                        await imageService.SaveResizedImage(environment.WebRootPath + DEFAULT_IMG_PATH, environment.WebRootPath + imgMini, 100, IMAGE_SUFFIX);
                        imgMini += IMAGE_SUFFIX;
                    }

                    user.Photo = imgSourse;
                    user.MediumImage = imgMedium;
                    user.MiniImage = imgMini;

                    var createResult = await userManager.CreateAsync(user, model.Password);

                    if (createResult.Succeeded)
                    {
                        var interests = new List<Interes>();
                        var splitted = tegSplitter.GetEnumerable(model.Tegs);
                        if (splitted != null)
                        {
                            foreach (var inter in splitted)
                            {
                                interests.Add(new Interes { Value = inter });
                            }
                            user.Intereses = interests;
                        }
                        user.MarketKibnet = new MarketKibnet { MarketState = MarketState.common, PaymentAccountBalance = 0, TotalMarcetCompanyCount = 0 };
                        context.Users.Update(user);
                        await context.SaveChangesAsync();
                        try
                        {
                            await SendEmailConfirmationAsync(model.Email);
                        }
                        catch (Exception ex)
                        {
                            await logger.LogObjectToFile("RegisterAccount", ex);
                        }
                        return View("ConfirmEmail", model.Email);
                    }
                    else
                    {
                        foreach (var identityError in createResult.Errors)
                        {
                            ModelState.AddModelError("", identityError.Description);
                        }
                    }
                }
                return View(model); 
            }
            catch(Exception ex)
            {
                await logger.LogStringToFile($"reg post error {ex.Message} {ex.StackTrace}");
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        /// <summary>
        /// Отправляет сообщение с подтверждением пароля на почту.
        /// </summary>
        /// <param name="email">Email к которому привязан аккаунт.</param>
        /// <returns></returns>
        [NonAction]
        public async Task SendEmailConfirmationAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = Url.Action("FinishConfirmEmail", "Account", new { token = token, userId = user.Id }, HttpContext.Request.Scheme);
            string message = $"<p>Внимаение! Этот адрес был указан при регистрации на сайте nisidi.ru. </p>" +
                $"<p>Для подтвердения адреса перейдите по ссылке нажав <a href=\"{url}\">ЗДЕСЬ</a>.</p>" +
                $"<p>Это письмо создано автоматически, пожалуйста не отвечайте на него.</p>" +
                $"<p>Если вы не регистрировались на сайте, то просто проигнорируйте это сообщение.</p>";

            var mailSender = new MailSender(logger);

            await mailSender.SendEmailAsync(email, "Подтверждение электронной почты на сайте nisidi.ru.", message);
        }
        /// <summary>
        /// Повторная отправка подтверждения почты.
        /// </summary>
        /// <param name="email">email на который зарегестрирован аккаунт.</param>
        /// <returns></returns>
        public async Task<IActionResult> RepeatEmailConfirmMessage(string email)
        {
            if(string.IsNullOrWhiteSpace(email))
                return View("ConfirmEmail", email);

            await SendEmailConfirmationAsync(email);

            return View("ConfirmEmail", email);
        }
        /// <summary>
        /// Финальное подтверждение почты. 
        /// </summary>
        /// <param name="token">тоен подтверждения</param>
        /// <param name="userId">id пользователя, которого подтверждать</param>
        /// <returns></returns>
        public async Task<IActionResult> FinishConfirmEmail(string token,string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var rezult = await userManager.ConfirmEmailAsync(user, token);
            Debug.WriteLine("regisration email confirm errors:", rezult.Errors);
            if (rezult.Succeeded)
            {
                await signInManager.SignInAsync(user, true);
                return RedirectToAction("Start", "Events");
            }
            else
                return RedirectToAction("Login");
        }

        /// <summary>
        /// Разлогин.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Exit()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Start", "Events");
        }
        /// <summary>
        /// Форма изменения пароля.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }
        /// <summary>
        /// Изменение пароля.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {           
            if (model.OldPassword == null || model.NewPassword == null || model.ConfirmPassword == null)
            {
                ModelState.AddModelError("", "Все поля обязательны для заполнения.");
                return View(model);
            }
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Новый пароль и подтверждение не совпадают.");
                return View(model);
            }
            var user = await userManager.GetUserAsync(User);
            var rezult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (rezult.Succeeded)
            {
                return RedirectToAction("Index", "MyPage");
            }
            else 
            {
                ModelState.AddModelError("", "Что-то пошло не так.");
                return View(model);
            }            
        }

        /// <summary>
        /// Начало восстановления пароля.
        /// </summary>
        /// <returns></returns>
        public IActionResult PasswordRecovery() => View();
        /// <summary>
        /// Начало восстановления пароля ввод почты.
        /// </summary>
        /// <param name="email">почта - логин для восстановления.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PasswordRecovery(string email) 
        {
            if(string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("", "Не указан адрес электронной почты");
                return View();
            }
            
            var user = await userManager.FindByNameAsync(email);
            var isConfirmed = !(await userManager.IsEmailConfirmedAsync(user));
            if (user==null || isConfirmed)
            {
                ViewBag.IsUserNull = user == null ? true: false;
                ViewBag.IsNotConfirmed = isConfirmed ? true: false;
                return View("PasswordRecoweryBadModel");
            }

            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action("PasswordRecoveryPage", "Account", new {userId= user.Id, code = code }, HttpContext.Request.Scheme);
            MailSender mailSender = new MailSender(logger);
            await mailSender.SendEmailAsync(email, "Восстановление пароля", $"<a href={url}>Нажмите для восстановления</a>");
            return View("PasswordRecoveryConfirm");
        }
        /// <summary>
        /// Страница с вводом нового пароля.
        /// </summary>
        /// <param name="userId">id пользователя.</param>
        /// <param name="code">сгенерированный токен для восстановления.</param>
        /// <returns></returns>
        public async Task<IActionResult> PasswordRecoveryPage(string userId, string code)
        {
            if(code!= null)
                return View(new PasswordRecoveryVM { UserId = userId, Token = code });
            else
                return BadRequest();
        }
        /// <summary>
        /// Восстановление пароля. 
        /// </summary>
        /// <param name="model"> одель с паролями и токеном и id пользоватеял лоя восстановления</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PasswordRecoveryPage(PasswordRecoveryVM model)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.UserId);
                // Возвращаем сюда чтоб не дать информаци злоумышленнику о том что именно не сработало.
                if (user == null) return View("PasswordRecoveryConfirm");

                var changeResult = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if(changeResult.Succeeded)
                {
                    return View("PasswordRecoveryConfirmed");
                }
                else
                {
                    foreach(var err in changeResult.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }
    }
}