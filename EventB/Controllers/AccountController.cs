using System;
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
using CommonServices.Infrastructure.Helpers;
using EventB.Services.AccountService;
using CommonServices.Infrastructure.WebApi;

namespace EventB.Controllers
{
    public class AccountController : Controller
    {
        private const string DEFAULT_IMG_PATH = "/images/defaultuser.jpg";
        private const string IMAGE_SUFFIX = ".jpeg";

        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly Context context;
        readonly IWebHostEnvironment environment;
        private readonly IImageService imageService;
        private readonly ITegSplitter tegSplitter;
        private readonly ILogger logger;

        private readonly IAccountService accountService;

        public AccountController(UserManager<User> UM,
            SignInManager<User> SIM,
            ITegSplitter TS,
            Context Context,
            IWebHostEnvironment _environment,
            ILogger _logger,
            IImageService _imageService,
            IAccountService _accountService
            )
        {
            userManager = UM;
            signInManager = SIM;
            tegSplitter = TS;
            context = Context;
            environment = _environment;
            logger = _logger;
            imageService = _imageService;
            accountService = _accountService;
        }

        public  IActionResult Login(string returnUrl, AccountModel model = null)
        {
            if(model == null)
            {
                model = new AccountModel();
            }
            return View(model);
        }

        [ValidateAntiForgeryToken, HttpPost]
        [Route("Account/Login")]
        public async Task<IActionResult> Login(AccountModel model)
        {
            model.IsRepeatLoading = true;
            if (ModelState.IsValid)
            {
                var user = await (userManager.FindByEmailAsync(model.Login));
                if(user == null)
                {
                    ModelState.AddModelError("", "Пользователь не найден или неверный пароль");
                    return View(model);
                }

                if (!await userManager.IsEmailConfirmedAsync(user))
                    return View("ConfirmEmail", model.Login);
                
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

        [ValidateAntiForgeryToken, HttpPost]
        [Route("Account/Register")]
        public async Task<IActionResult> Register(AccountModel model)
        {
            try
            {
                if (!model.AgreePersonalData)
                {
                    ModelState.AddModelError("", "Необходимо согласится на обработку персональных данных");
                }
                if (model.Login == null || string.IsNullOrEmpty(model.Login.Trim()))
                {
                    ModelState.AddModelError("", "Поле ЛОГИН не может быть пустым");
                }
                if (model.City == null || string.IsNullOrEmpty(model.City.Trim()))
                {
                    ModelState.AddModelError("", "Поле ГОРОД не может быть пустым");
                }

                if (ModelState.IsValid)
                {
                    var user = new User()
                    {
                        Email = model.Login,
                        UserName = model.Login,
                        Name = model.UserName,
                        NormalizedName = model.UserName.ToUpper(),
                        City = model.City.Trim(),
                        NormalizedCity = model.City.Trim().ToUpper()
                    };

                    user.MarketKibnet = new MarketKibnet { MarketState = MarketState.common, PaymentAccountBalance = 0, TotalMarcetCompanyCount = 0 };

                    // Изображение
                    var fileName = Guid.NewGuid().ToString();
                    string imgSourse = String.Concat("/images/Profileimages/", fileName);
                    string imgMedium = String.Concat("/images/Profileimages/Medium/", "M" + fileName);
                    string imgMini = String.Concat("/images/Profileimages/Mini/", "m" + fileName);

                    await imageService.SaveResizedImage(environment.WebRootPath + DEFAULT_IMG_PATH, environment.WebRootPath + imgSourse, 500, IMAGE_SUFFIX);
                    imgSourse += IMAGE_SUFFIX;
                    await imageService.SaveResizedImage(environment.WebRootPath + DEFAULT_IMG_PATH, environment.WebRootPath + imgMedium, 360, IMAGE_SUFFIX);
                    imgMedium += IMAGE_SUFFIX;
                    await imageService.SaveResizedImage(environment.WebRootPath + DEFAULT_IMG_PATH, environment.WebRootPath + imgMini, 100, IMAGE_SUFFIX);
                    imgMini += IMAGE_SUFFIX;

                    user.Photo = imgSourse;
                    user.MediumImage = imgMedium;
                    user.MiniImage = imgMini;

                    // Создание пользователя

                    var createResult = await userManager.CreateAsync(user, model.Password);

                    if (createResult.Succeeded)
                    {
                        try
                        {
                            await SendEmailConfirmationAsync(model.Login, model.Password);
                        }
                        catch (Exception ex)
                        {
                            await logger.LogObjectToFile($"RegisterAccount error/ SendMailError ({model.Login} {model.Password})", ex);
                        }
                        return View("ConfirmEmail", model.Login);
                    }
                    else
                    {
                        foreach (var identityError in createResult.Errors)
                        {
                            ModelState.AddModelError("", identityError.Description);
                        }
                    }
                }
                model.IsRepeatLoading = true;
                model.LoadRegisterPage = true;
                return View("Login", model);
            }
            catch(Exception ex)
            {
                model.IsRepeatLoading = true;
                model.LoadRegisterPage = true;
                await logger.LogStringToFile($"reg post error {ex.Message} {ex.StackTrace}");
                ModelState.AddModelError("", ex.Message);
                return View("Login", model);
            }
        }

        /// <summary>
        /// Отправляет сообщение с подтверждением пароля на почту.
        /// </summary>
        /// <param name="email">Email к которому привязан аккаунт.</param>
        /// <returns></returns>
        [NonAction]
        public async Task SendEmailConfirmationAsync(string email, string userSecret = null)
        {
            var user = await userManager.FindByEmailAsync(email);
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = Url.Action("FinishConfirmEmail", "Account", new { token = token, userId = user.Id }, HttpContext.Request.Scheme);

            var stringWithSecret = $"<p>Ваш пароль <span style=\"font-size:20px; color:#1b5972; margin:0 1em;\">{userSecret}<span></p>";

            string message =
                $"<h3>Добро пожаловать на </span style=\"color:#1b5972; margin:0 1em;\">NISIDI.RU</span> </h3>" +
                $"<p>Внимаение! Этот адрес был указан при регистрации на сайте nisidi.ru. </p>";
            if(userSecret != null)
            {
                message += stringWithSecret;
            }
            message +=
                $"<p>Для подтвердения адреса перейдите по ссылке нажав <a style=\"font-size:20px; color:#1b5972;\" href=\"{url}\">ЗДЕСЬ</a>.</p><br>" +
                $"<p>После подтверждения вы сможете создавать свои события, сохранять свои интересы, находить друзей и общаться с другими пользователями.</p><br>" +
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
                return View("PasswordRecoveryConfirmed");
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
                ViewBag.Email = email;
                return View();
            }
            
            var user = await userManager.FindByNameAsync(email);
            var userIsConfirmed = await userManager.IsEmailConfirmedAsync(user);
            if (user==null || !userIsConfirmed)
            {
                ViewBag.Email = email;
                if(user == null)
                {
                    ModelState.AddModelError("", "Пользователь не найден");
                }
                if (!userIsConfirmed)
                {
                    ModelState.AddModelError("", "Пользователь не подтвержден. Перейдите по ссылке из письма подтверждения.");
                }
                
                return View("PasswordRecovery");
            }

            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action("PasswordRecoveryPage", "Account", new {userId= user.Id, code = code }, HttpContext.Request.Scheme);

            var message = "<h3>Восстановление пароля на сайте nisidi.ru</h3>";
            message += $"<p>Для перехода на страницу восстановления пароля перейдите по <a href={url} style=\"font-size:20px; color:#1b5972;\">ССЫЛКЕ</a></p>";

            MailSender mailSender = new MailSender(logger);
            await mailSender.SendEmailAsync(email, "Восстановление пароля най сайте nisidi.ru", message);
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
               
                if (changeResult.Succeeded)
                {
                    if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                    {
                        await signInManager.PasswordSignInAsync(user, model.Password, true, false);
                    }
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

        /// <summary>
        /// Генерация случайного пароля
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetNewRandomPassword()
        {
            return StringHelper.GetAccountPassword(8);
        }

        #region Содержимое пользователя

        /// <summary>
        /// Сохранить новый интерес
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<WebResponce<bool>> SaveUserInteres(string value)
        {
            try
            {
                if(!User.Identity.IsAuthenticated)
                {
                    throw new Exception("Пользователь не аутентифицирован. Сюда не должен был попасть");
                }
                await accountService.AddInteresTouser(User.FindFirstValue(ClaimTypes.NameIdentifier), value);
                return new WebResponce<bool>(true);
            }
            catch(Exception ex)
            {
                await logger.LogStringToFile($"Ошибка AcccountController.SaveUserInteres {ex.Message}\n{ex.StackTrace}");
                return new WebResponce<bool>(false, false, ex.Message);
            }
        }

        /// <summary>
        /// Удалить интерес
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<WebResponce<bool>> DeleteUserInteres(string value)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    throw new Exception("Пользователь не аутентифицирован. Сюда не должен был попасть");
                }
                await accountService.DeleteInteresForUser(User.FindFirstValue(ClaimTypes.NameIdentifier), value);
                return new WebResponce<bool>(true);
            }
            catch (Exception ex)
            {
                await logger.LogStringToFile($"Ошибка AcccountController.SaveUserInteres {ex.Message}\n{ex.StackTrace}");
                return new WebResponce<bool>(false, false, ex.Message);
            }
        }
        #endregion


    }
}