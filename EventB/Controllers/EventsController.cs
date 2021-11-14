using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EventBLib.Models;
using EventBLib.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EventB.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using EventB.ViewModels;
using System.Threading;
using EventB.ViewModels.EventsVM;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using EventB.Services.EventServices;
using EventB.ViewModels.SharedViews;
using CommonServices.Infrastructure.WebApi;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace EventB.Controllers
{
    public class EventsController : Controller
    {
        private Context context { get; }
        private readonly IEventService eventService;
        private readonly SignInManager<User> signInManager;
        readonly UserManager<User> userManager;
        readonly IUserFindService userFindService;
        IWebHostEnvironment environment;
        ITegSplitter tegSplitter;
        IEventSelectorService eventSelector;
        private readonly ILogger<EventsController> _logger;

        public EventsController(Context c,
            IEventService _eventService,
            SignInManager<User> UM,
            IWebHostEnvironment env,
            IUserFindService _userFindService,
            ITegSplitter _tegSplitter,
            IEventSelectorService _eventSelector,
             UserManager<User> _userManager,
             ILogger<EventsController> logger)
        {
            context = c;
            eventService = _eventService;
            signInManager = UM;
            userManager = _userManager;
            environment = env;
            userFindService = _userFindService;
            tegSplitter = _tegSplitter;
            eventSelector = _eventSelector;
            _logger = logger;
        }
        #region Выборки и постраничный вывод
        /// <summary>
        /// Начальная страница с параметрами входа
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Start()
        {
            // Сформировать параметры для будущего поиска.
            if (User.Identity.IsAuthenticated)
            {
                var user = await context.Users.Include(e=>e.Intereses).FirstAsync(e=>e.UserName == User.Identity.Name);
                var tegs = user.Intereses.ToList();
                var tegsStr = "";
                foreach (var e in tegs)
                {
                    tegsStr = $"{tegsStr} {e.Value}";
                }
                var args = new CostomSelectionArgs { 
                        DateSince = DateTime.Now,
                        DateDue = DateTime.Now.AddDays(90),
                        Title = "",
                        City = user.City,
                        Tegs = "",
                        IsTegsFromProfile = true,
                        Skip=0,
                        Take=30
                };
                
                var VM = new EventListVM
                {
                    events = null,
                    args = args,
                    Tegs = null,
                    ByMyTegsLink = GetByMyTegsLink(user),
                    UserInterses = new UserIntereses()
                    {
                        UserId = user.Id,
                        Intereses = user.Intereses.Select(e=>e.Value).ToList()
                    }
                };
                return View(VM);
            }
            else
            {
                var args = new CostomSelectionArgs
                {
                    DateSince = DateTime.Now,
                    DateDue = DateTime.Now.AddDays(90),
                    Title = "",
                    City = "СТАВРОПОЛЬ",
                    Tegs = "",
                    IsTegsFromProfile = false,
                    Skip = 0,
                    Take = 30
                };
                
                var VM = new EventListVM
                {
                    events = null,
                    args = args,
                    ByMyTegsLink = null,
                    UserInterses = new UserIntereses()
                };
                return View(VM);
            }
        }

        /// <summary>
        /// Динамическая загрузка для постраничного вывода.
        /// </summary>
        /// <param name="args">Аргументы для ффильтраци.</param>
        /// <returns></returns>
        [Route("/Events/LoadDynamic")]
        public async Task<IActionResult> LoadDynamic(CostomSelectionArgs args) 
        {
            var events = await eventSelector.GetCostomEventsAsync(args);

            var banners = new Dictionary<int, string>();

            if (args.Skip == 0 && events.Count == 0)
            {
                banners.Add(-1, "~/Views/Banner/_EmptySearchResultLeadMagnet.cshtml");
                Response.StatusCode = 206;
            } 

            if(events.Count == 0 || events.Count < args.Take) {
                Response.StatusCode = 206;
            }

            if (!User.Identity.IsAuthenticated)
            {
                banners.Add(6, "~/Views/Banner/_RegisterLeadMagnet.cshtml");
            }
            else
            {
                banners.Add(6, "~/Views/Banner/_AddEventForAuthorizedEventList.cshtml");
            }
            
            var model = new EventSerchResultBatch()
            {
                Events = events,
                TemplatesWithPositions = banners
            };
            return PartialView("_eventListPartial", model);
        }

        [Route("/Events/SearchEventlist")]
        public async Task<IActionResult> SearchEventlist(CostomSelectionArgs args)
        {
            User user;
            string url;
            UserIntereses userIntereses;

            if (User.Identity.IsAuthenticated)
            {
                user = await context.Users.Include(e => e.Intereses).FirstAsync(e => e.UserName == User.Identity.Name);
                url = GetByMyTegsLink(user);
                userIntereses = new UserIntereses()
                {
                    Intereses = user.Intereses.Select(e => e.Value).ToList(),
                    UserId = user.Id
                };
            }
            else
            {
                user = null;
                url = null;
                userIntereses = new UserIntereses();
            }
            
            args.DateDue = args.DateDue.AddDays(1).AddMinutes(-1);
            args.Title = args.Title ?? "";

            args.City = args.City != null ? args.City.ToUpper() : (user!=null ? user.NormalizedCity : "СТАВРОПОЛЬ");
            args.Tegs = args.Tegs ?? "";
            
            var VM = new EventListVM
            {
                events = null,
                args = args,
                Tegs = tegSplitter.GetEnumerable(args.Tegs),
                ByMyTegsLink = url,
                UserInterses = userIntereses
            };
            return View("Start", VM);
        }
        #endregion

        [Authorize]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [Route("Events/Add")]
        public async Task<IActionResult> Add(AddEventViewModel model)
        {
            _logger.LogError($"Add event {model.Title}");
            if (ModelState.IsValid)
            {
                var eve = await eventService.AddEvent(model, User.Identity.Name);
                return Redirect($"/Events/Details/{eve.EventId}");
            }
            else
            {
                ModelState.AddModelError("", "Неверные данные");
                return View(model);
            }
        }

        /// <summary>
        /// Страница детали события
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Details(int id)
        {             
            Event eve;

            eve = await eventService.Details(id);

            if(eve == null)
            {
                return View("~/Views/Shared/_AccessDeniedPage.cshtml", new AccessDeniedVM()
                {
                    Tittle = "Искомое событие не найдено",
                    Reazon = "Событие с введенными параметрами не найдено.",
                    Text = "Попробуйте изменить параметры поиска и проверить правильность ввода адреса"
                });
            }
            UserChat userChat;
            if (!string.IsNullOrWhiteSpace(User?.Identity?.Name))
            {
                var user = await context.Users.
                    Include(e=>e.Invites).
                    Include(e=>e.Friends).
                    FirstAsync(e=>e.NormalizedUserName == User.Identity.Name.ToUpper());
                ViewData["UserId"] = user.Id;
                ViewData["UserName"] = user.Name;

                userChat = new UserChat();

                if(eve.Type == EventType.Private && 
                    !eve.Vizits.Any(e=>e.UserId == user.Id) && 
                    !user.Invites.Any(e=>e.EventId == id) && 
                    !user.Friends.Any(e => !e.IsBlocked && e.FriendUserId == eve.UserId) &&
                    eve.UserId != user.Id)
                {
                    return View("~/Views/Shared/_AccessDeniedPage.cshtml", new AccessDeniedVM()
                    {
                        Tittle = "Недоступно",
                        Reazon = "Событие является привантым.",
                        Text = "Для просмотра добавьте организатора в дузья или получите приглашение на это событие."
                    });
                }
            }
            else
            {
                userChat = new UserChat();
                ViewData["UserId"] = "0";
                ViewData["UserName"] = "Неавторизован";
                if (eve.Type == EventType.Private)
                {
                    return View("~/Views/Shared/_AccessDeniedPage.cshtml", new AccessDeniedVM()
                    {
                        Tittle = "Недоступно",
                        Reazon = "Событие является привантым.",
                        Text = "Необходима регистрация для просмотра частных событий. Для просмотра добавьте организатора в друзья или получите приглашение на это событие."
                    });
                }
            }
            eve.Chat.UserChat = new List<UserChat>() { userChat};
            return View(eve);
        }

        #region АПИ для деталей события
        /// <summary>
        /// Получение истории изменений.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [Route("/Evetns/GetEventMessages"), HttpGet]
        public async Task<List<Message>> GetEventMessages(int eventId)
        {            
            return await eventService.GetEventMessages(eventId);
        }
        
        /// <summary>
        /// Отправка сообщения в чат события.
        /// </summary>
        /// <param name="userId">id пользователя</param>
        /// <param name="chatId">id чата</param>
        /// <returns></returns>
        [Authorize]
        [Route("/Events/SendMessage")]
        public async Task<StatusCodeResult> SendMessage(string userId, string userName, int chatId, string text)
        {
            var user = await context.Users.FirstAsync(e => e.UserName == User.Identity.Name);
            var result = await eventService.SendMessage(user.Id, user, chatId, text);
            return StatusCode(result);
        }

        /// <summary>
        /// Добавить пользователя как участника события.
        /// </summary>
        /// <param name="eventId">Id события</param>
        /// <returns></returns>
        [Route("/Event/SubmitToEvent")]
        public async Task<WebResponce<string>> SubmitToEvent(int eventId)
        {
            if (User?.Identity?.Name == null) return new WebResponce<string>("not ok", false, "Неавторизован");
            // !! не создавать UserChat если он уже есть
            // Поверить
            // ВАЖНАЯ функция
            var code = await eventService.SubmitToEvent(eventId, User.Identity.Name);
            return new WebResponce<string>("ОК");
        }

        /// <summary>
        /// Отправка ссылки на событие в чат.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [Authorize]
        [Route("/Event/SendLink")]
        public async Task<StatusCodeResult> SendLink(int eventId, int userChatId)
        {
            return StatusCode( await eventService.SendLink(eventId, userChatId, User.Identity.Name));
        }

        /// <summary>
        /// Возвращает список доступных чатов виде PV _chatSmallListPartial
        /// ИСКЛЮЧЕНЫ ЧАТЫ СОБЫТИЙ
        /// </summary>
        /// <returns></returns>
        [Route("/Event/GetAvailableChats")]
        public async Task<PartialViewResult> GetAvailableChats()
        {
            //if (User?.Identity?.Name == null) return new WebResponce<PartialViewResult>(null, false, "Not authorize");
            if (User?.Identity?.Name == null) throw new Exception("Not authorize");

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var chats = await context.UserChats.Include(e => e.Chat).Where(e => e.UserId == user.Id).ToListAsync();
            chats = chats.Where(e => e.Chat.EventId == null && e.IsBlockedInChat == false).ToList();
            return PartialView("_chatSmallListPartial", chats);
        }
        #endregion

        #region Форма приглашений на событие
        /// <summary>
        /// Возвращает список друзей которых можно пригласить.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task<WebResponce<List<InviteOutVM>>> GetFriendslist(int eventId)
        {
            if (User?.Identity?.Name == null) { 
                var ans = new WebResponce<List<InviteOutVM>>(null, false, "not authorize");
                return ans;
            }
            return new WebResponce<List<InviteOutVM>>(await eventService.GetFriendslist(eventId, User.Identity.Name));
        }

        /// <summary>
        /// Апи получает приглашения с морды и парсит их в Invite.
        /// </summary>
        /// <param name="eventId">Ид события</param>
        /// <param name="invites">Инвайты (userId - id, message- сообщение)</param>
        /// <returns></returns>
        public async Task InviteFriendsIn(int eventId, InviteInVm[] invites)
        {
            // Вставить проверку на то что чел друг а не левый
            await eventService.InviteFriendsIn(eventId, User.Identity.Name, invites);
        }
        #endregion

        #region изменение события
        /// <summary>
        /// Страница редактирования события.
        /// </summary>
        /// <param name="eventId">Id события</param>
        /// <returns>Вид.</returns>
        [Authorize]
        [Route("Events/EventEdit")]
        public async Task<IActionResult> EventEdit(int eventId)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var eve = await context.Events.Include(e => e.EventTegs).FirstOrDefaultAsync(e => e.EventId == eventId);
            if (userId == null || eve == null)
            {
                Response.StatusCode = 204;
                ViewBag.Status = 204;
                return View(null);
            }
            if (userId != eve.UserId)
            {
                Response.StatusCode = 403;
                ViewBag.Status = 403;
                return View(null);
            }
            var model = new EventB.ViewModels.EventsVM.EventEditVM
            {
                Title = eve.Title,
                OldTitle = eve.Title,
                Tegs = eve.Tegs,
                OldTegs = eve.Tegs,
                Body = eve.Body,
                OldBody = eve.Body,
                Date = eve.Date,
                OldDate = eve.Date,
                City = eve.City,
                OldCity = eve.City,
                Place = eve.Place,
                OldPlace = eve.Place,
                MainPicture = eve.Image,
                EventId = eve.EventId,
                Tickets = eve.TicketsDesc,
                OldTickets = eve.TicketsDesc,
                Phone = eve.Phone,
                OldPhone = eve.Phone,
                AgeRest = eve.AgeRestrictions
            };
            ViewBag.EventType = eve.Type;
            return View(model);
        }

        /// <summary>
        /// Внесение изменений в событие.
        /// </summary>
        /// <param name="model">VM формы представления события</param>
        /// <returns>редирект к деталям события</returns>
        [Authorize]
        [HttpPost]
        [Route("Events/EventEdit")]
        public async Task<IActionResult> EventEdit(EventEditVM model)
        {
            // Валидация.
            if ( string.IsNullOrWhiteSpace(model.City) ||  string.IsNullOrWhiteSpace(model.Title))
            {
                ModelState.AddModelError("", "Нужно заполнить обязательные поля");
                return View(model);
            }
            else
            {
                var eve = await eventService.EventEdit(User.Identity.Name, model);
                return Redirect($"/Events/Details/{eve.EventId}");
            }        
        }

        /// <summary>
        /// Удаление события.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [Authorize]
        [Route("Events/DeleteEvent")]
        public async Task<IActionResult> DeleteEvent(int eventId)
        {
            var result = await eventService.DeleteEvent(User.Identity.Name, eventId);
            if(result == 200) return RedirectToAction("Index", "MyPage");
            else return StatusCode(result);
        }
        #endregion

        [Authorize]
        public async Task<string> GetLinqToFilterByTegs()
        {
            var user = await context.Users.Include(e => e.Intereses).FirstOrDefaultAsync(e => e.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            return GetByMyTegsLink(user);
        }

        private string GetByMyTegsLink(User user)
        {
            return $"/Events/SearchEventlist?City={user.NormalizedCity}&Tegs={string.Join("@", user.Intereses.Select(e => e.Value).ToList())}&Skip=0&Take=30";
        }
    }
}