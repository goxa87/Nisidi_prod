using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EventB.Services;
using EventB.Services.EventServices;
using EventB.Services.ImageService;
using EventB.ViewModels.MyPage;
using EventB.ViewModels.Paging;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventB.Controllers
{
    /// <summary>
    /// Моя страница.
    /// </summary>
    [Authorize]
    public class MyPageController : Controller
    {
        private const string IMAGE_SUFFIX = ".jpeg";

        readonly Context context;
        readonly UserManager<User> userManager;
        readonly ITegSplitter tegSplitter;
        readonly IWebHostEnvironment environment;
        private readonly IImageService imageService;
        private readonly IEventService _eventService;
        private readonly SettingsService _settingsService;

        public MyPageController(Context _context,
            UserManager<User> _userManager,
            ITegSplitter _tegSplitter,
            IWebHostEnvironment _environment,
            IImageService _imageService,
            IEventService eventService,
            SettingsService settingsService)
        {
            context = _context;
            userManager = _userManager;
            tegSplitter = _tegSplitter;
            environment = _environment;
            imageService = _imageService;
            _eventService = eventService;
            _settingsService = settingsService;
        }
        /// <summary>
        /// Страница профиля.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var user = await context.Users.
                Include(e => e.Intereses).
                FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);

            return View(user);
        }

        /// <summary>
        /// Получить разметку для вкладки пойду 
        /// (Рендеринг для первоначальной загрузки. Для подгрузки страниц смотри следующий метод)
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("/MyPage/GetVizits")]
        public async Task<IActionResult> GetVizitsMarkup(string filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var events = await _eventService.GetUserVizitsEvents(userId, filter);

            var itemsCount = events.Count;
            var model = new EventsPagingFilterModel<Event, string>()
            {
                Events = events.Take(_settingsService.DefaultPagingPageSize).ToList(),
                Paging = new PagingBaseModel(itemsCount, 1, _settingsService.DefaultPagingPageSize, "mp-tab-vizit-paging"),
                Filter = filter
            };
            return PartialView("~/Views/MyPage/Partials/_MyPageTabVizits.cshtml", model);
        }

        /// <summary>
        /// Получить разметку для страницы пойду 
        /// (Рендеринг Конкретной страницы с номером)
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("/MyPage/GetVizitsPage")]
        public async Task<IActionResult> GetVizitsMarkup(int currentPage, string filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var events = await _eventService.GetUserVizitsEvents(userId, filter);

            var itemsCount = events.Count;
            var model = new EventsPagingFilterModel<Event, string>()
            {
                Events = events.Skip((currentPage-1)*_settingsService.DefaultPagingPageSize).Take(_settingsService.DefaultPagingPageSize).ToList(),
                Paging = new PagingBaseModel(itemsCount, currentPage, _settingsService.DefaultPagingPageSize, "mp-tab-vizit-paging"),
                Filter = filter
            };
            return PartialView("~/Views/MyPage/Partials/_MyPageVizitsPage.cshtml", model);
        }

        /// <summary>
        /// Получить разметку для вкладки созданных
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("/MyPage/GetCreatedTab")]
        public async Task<IActionResult> GetEventsMarkup(string filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var events = await _eventService.GetUserCreatedEvents(userId, filter);

            var itemsCount = events.Count;
            var model = new EventsPagingFilterModel<Event, string>()
            {
                Events = events.Take(_settingsService.DefaultPagingPageSize).ToList(),
                Paging = new PagingBaseModel(itemsCount, 1, _settingsService.DefaultPagingPageSize, "mp-tab-created-paging"),
                Filter = filter
            };

            return PartialView("~/Views/MyPage/Partials/_MyPageTabCreated.cshtml", model);
        }

        /// <summary>
        /// Получить разметку для вкладки созданных
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("/MyPage/GetCreatedPage")]
        public async Task<IActionResult> GetCteatedPage(int currentPage, string filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var events = await _eventService.GetUserCreatedEvents(userId, filter);

            var itemsCount = events.Count;
            var model = new EventsPagingFilterModel<Event, string>()
            {
                Events = events.Skip((currentPage - 1) * _settingsService.DefaultPagingPageSize).Take(_settingsService.DefaultPagingPageSize).ToList(),
                Paging = new PagingBaseModel(itemsCount, currentPage, _settingsService.DefaultPagingPageSize, "mp-tab-created-paging"),
                Filter = filter
            };

            return PartialView("~/Views/MyPage/Partials/_MyPageVizitsPage.cshtml", model);
        }

        /// <summary>
        /// Получить разметку для вкладки приглашений
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("/MyPage/GetInvites")]
        public async Task<IActionResult> GetInvitesMarkup()
        {
            var user = await context.Users.Include(e => e.Invites).ThenInclude(e=>e.Event).FirstAsync(e=>e.UserName == User.Identity.Name);
            var invites = user.Invites;

            return PartialView("~/Views/MyPage/Partials/_MyPageTabInvites.cshtml", invites);
        }

        /// <summary>
        /// Подтверждение приглашения.
        /// </summary>
        /// <param name="eventId">Ид события.</param>
        /// <param name="inviteId">Ид приглашения.</param>
        /// <returns></returns>
        [Route("/MyPage/SubmitInvite")]
        public async Task<StatusCodeResult> SubmitInvite(int eventId, int inviteId)
        {
            var curUser = await userManager.FindByNameAsync(User.Identity.Name);
            var eve = await context.Events.Include(e => e.Chat).FirstOrDefaultAsync(e => e.EventId == eventId);
            var inv = await context.Invites.FirstOrDefaultAsync(e => e.InviteId == inviteId);
            if (curUser.Id != inv.UserId)
            {
                return StatusCode(401);
            }
            if (curUser == null || eve == null || inv == null)
            {
                return StatusCode(410);
            }
            // Если визит уже есть
            if (await context.Vizits.AnyAsync(e => e.EventId == eventId && e.UserId == curUser.Id))
            {
                return Ok();
            }
            // Добавляем Визит.
            var newVizit = new Vizit
            {
                UserId = curUser.Id,
                EventId = eve.EventId,
                EventTitle = eve.Title,
                EventPhoto = eve.MediumImage,
                VizitorName = curUser.Name,
                VizitirPhoto = curUser.MediumImage
            };
            context.Vizits.Add(newVizit);
            var userChat = new UserChat
            {
                ChatId = eve.Chat.ChatId,
                ChatName = eve.Title,
                ChatPhoto = eve.MiniImage,
                UserId = curUser.Id,
                SystemUserName = curUser.UserName,
                RealtedObjectLink = $"/Events/Details/{eve.EventId}"
            };
            context.UserChats.Add(userChat);
            // Удаляем приглашение.
            context.Invites.Remove(inv);
            await context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Отказ от приглашения.
        /// </summary>
        /// <param name="inviteId">Ид приглашения.</param>
        /// <returns></returns>
        [Route("/MyPage/RefuseInvite")]
        public async Task RefuseInvite(int inviteId)
        {
            var inv = await context.Invites.FirstOrDefaultAsync(e => e.InviteId == inviteId);
            if (inv == null)
            {
                Response.StatusCode = 410;
                return;
            }
            context.Invites.Remove(inv);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Заполнение формы для редактирования профиля.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("MyPage/EditProfile")]
        public async Task<IActionResult> EditProfile()
        {
            var curUser = await userManager.GetUserAsync(HttpContext.User);
            var tegs = await context.Intereses.Where(e => e.UserId == curUser.Id).ToListAsync();
            var tegsStr = "";
            foreach (var teg in tegs)
            {
                tegsStr = $"{tegsStr} {teg.Value}";
            }
            var VM = new EditProfileVM
            {
                Name = curUser.Name,
                OldName = curUser.Name,
                OldPhoto = curUser.Photo,
                City = curUser.City,
                Tegs = tegsStr,
                OldTegs = tegsStr,
                Description = curUser.Description,
                AlowAnonMessages = curUser.AnonMessages,
                Visibility = curUser.Visibility,
                PhoneNumber = curUser.PhoneNumber
            };

            return View(VM);
        }
        /// <summary>
        /// Редактирование профиля.
        /// </summary>
        /// <param name="model">VM от пользователя.</param>
        /// <returns>Редирект к моей странице.</returns>
        [HttpPost]
        [Route("MyPage/EditProfile")]
        public async Task<IActionResult> EditProfile(EditProfileVM model)
        {
            if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.City))
            {
                ModelState.AddModelError("", "Заполните поля Имя и Город");
                return View(model);
            }
            else
            {

                // TODO FRIEND
                // Сдесь всю шляпу получать за раз что нужно менять
                var user = await context.Users
                    .Include(e => e.Vizits)
                    .FirstOrDefaultAsync(e => e.UserName == HttpContext.User.Identity.Name);
                List<Friend> inFriends;
                List<Invite> inInvites = await context.Invites.Where(e => e.InviterId == user.Id).ToListAsync();
                List<Vizit> inVizits = user.Vizits;
                // Это юзер чаты в которых участвует, но привязвны к другим пользователям (Приватые). 
                List<UserChat> inUserChats = await context.UserChats.Where(e => e.OpponentId == user.Id).ToListAsync();

                if (model.newPhoto != null)
                {
                    var userImgDict = new Dictionary<int, string>();
                    userImgDict.Add(400, TrimSuffix(user.Photo));
                    userImgDict.Add(360, TrimSuffix(user.MediumImage));
                    userImgDict.Add(100, TrimSuffix(user.MiniImage));

                    await imageService.SaveOriginAndResizedImagesByInputedSizes(model.newPhoto, IMAGE_SUFFIX, userImgDict, null);
                } 

                // Изменения имени в таблицах.
                if (model.Name != model.OldName)
                {
                    user.Name = model.Name;
                    user.NormalizedName = model.Name.ToUpper();
                    inFriends = await context.Friends.Where(e => e.FriendUserId == user.Id).ToListAsync();
                    foreach (var e in inFriends)
                    {
                        e.UserName = model.Name;
                    }
                    foreach (var e in inInvites)
                    {
                        e.InviterName = model.Name;
                    }
                    foreach (var e in inVizits)
                    {
                        e.VizitorName = model.Name;
                    }                 
                    foreach(var e in inUserChats)
                    {
                        e.ChatName = model.Name;
                    }
                    foreach(var e in await context.Messages.Where(e=>e.PersonId == user.Id).ToListAsync())
                    {
                        e.SenderName = model.Name;
                    }
                }

                // Изменение интересов.
                if (model.Tegs != model.OldTegs)
                {
                    var oldTegs = context.Intereses.Where(e => e.UserId == user.Id);
                    context.RemoveRange(oldTegs);

                    var newTegs = new List<Interes>();
                    if(!string.IsNullOrEmpty(model.Tegs) && !string.IsNullOrWhiteSpace(model.Tegs))
                    {
                        var splitted = tegSplitter.GetEnumerable(model.Tegs)
                           .Select(e => new Interes { Value = e })
                           .ToList();
                        newTegs = splitted;
                    }

                    user.Intereses = newTegs;
                }

                // Прочие изменения.
                if (!string.IsNullOrEmpty(model.City))
                {
                    user.City = model.City.Trim();
                    user.NormalizedCity = model.City.Trim().ToUpper();
                }
                user.Description = model.Description;
                user.AnonMessages = model.AlowAnonMessages;
                user.Visibility = model.Visibility;
                user.PhoneNumber = model.PhoneNumber;
                context.Users.Update(user);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }

        #region private
        /// <summary>
        /// Вернет строчку без расширения файла
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string TrimSuffix(string path)
        {
            return path.Substring(0, path.LastIndexOf('.'));
        }
        #endregion
    }
}