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

        public EventsController(Context c,
            IEventService _eventService,
            SignInManager<User> UM,
            IWebHostEnvironment env,
            IUserFindService _userFindService,
            ITegSplitter _tegSplitter,
            IEventSelectorService _eventSelector,
             UserManager<User> _userManager)
        {
            context = c;
            eventService = _eventService;
            signInManager = UM;
            userManager = _userManager;
            environment = env;
            userFindService = _userFindService;
            tegSplitter = _tegSplitter;
            eventSelector = _eventSelector;
        }
        #region Выборки и постраничный вывод
        /// <summary>
        ///  Точка входа
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Start()
        {
            // Сформировать первоначальный список событий и задать параметры для будущего поиска.
            if (User.Identity.IsAuthenticated)
            {
                var user = await userManager.GetUserAsync(HttpContext.User);
                var tegs = await context.Intereses.Where(e => e.UserId == user.Id).ToListAsync();
                var tegsStr = "";
                foreach (var e in tegs)
                {
                    tegsStr = $"{tegsStr} {e.Value}";
                }
                var args = new CostomSelectionArgs { 
                        DateSince = DateTime.Now,
                        DateDue = DateTime.Now.AddDays(31),
                        Title = "",
                        City = user.City,
                        Tegs = tegsStr,
                        IsTegsFromProfile = true,
                        Skip=0,
                        Take=12
                    };
                //var rezult = await eventSelector.GetCostomEventsAsync(args);
                // Пропускаем то что уж нашли.
                //args.Skip += args.Take;
                var VM = new EventListVM
                {
                    events = null,
                    args = args
                };
                return View(VM);
            }
            else
            {
                var args = new CostomSelectionArgs
                {
                    DateSince = DateTime.Now,
                    DateDue = DateTime.Now.AddDays(31),
                    Title = "",
                    City = "МОСКВА",
                    Tegs = "",
                    IsTegsFromProfile = false,
                    Skip = 0,
                    Take = 12
                };
                //var rezult = await eventSelector.GetCostomEventsAsync(args);
                // Пропускаем то что уж нашли.
                //args.Skip += args.Take; 
                var VM = new EventListVM
                {
                    events = null,
                    args = args
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
            var rezult = await eventSelector.GetCostomEventsAsync(args);
            if (rezult != null && rezult.Any())
                return PartialView("_eventListPartial", rezult);
            else
                return StatusCode(204);
        }

        public async Task<IActionResult> SearchEventlist(CostomSelectionArgs args)
        {
            var user = await userManager.GetUserAsync(User);

            args.DateDue = args.DateDue.AddDays(1);
            args.Title = args.Title ?? "";

            args.City = args.City != null ? args.City.ToUpper() : (user!=null ? user.NormalizedCity : "Москва");
            args.Tegs = args.Tegs ?? "";

            //var rezult = await eventSelector.GetCostomEventsAsync(args);
            // Пропускаем то что уж нашли.
            //args.Skip += args.Take;
            
            var VM = new EventListVM
            {
                events = null,
                args = args
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
        public async Task<IActionResult> Add(AddEventViewModel model)
        {
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
        /// вызов деталей event по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Details(int? id)
        {
            if (id.HasValue)
            {
                var eve = await eventService.Details(id.Value);
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    ViewData["UserId"] = user.Id;
                    ViewData["UserName"] = user.Name;
                }
                else
                {
                    ViewData["UserId"] = "0";
                    ViewData["UserName"] = "Неавторизован";
                }
                return View(eve);
            }
            return NotFound();
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
            var result = await eventService.SendMessage(userId, userName, chatId, text);
            return StatusCode(result);
        }

        /// <summary>
        /// Добавить пользователя как участника события.
        /// </summary>
        /// <param name="eventId">Id события</param>
        /// <returns></returns>
        [Authorize]
        [Route("/Event/SubmitToEvent")]
        public async Task<StatusCodeResult> SubmitToEvent(int eventId)
        {
            // !! не создавать UserChat если он уже есть
            // Поверить
            // ВАЖНАЯ функция
            var code = await eventService.SubmitToEvent(eventId, User.Identity.Name);
            return StatusCode(code);
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
        [Authorize]
        [Route("/Event/GetAvailableChats")]
        public async Task<PartialViewResult> GetAvailableChats()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var chats = await context.UserChats.Include(e => e.Chat).Where(e => e.UserId == user.Id).ToListAsync();
            chats = chats.Where(e => e.Chat.EventId == null).ToList();
            return PartialView("_chatSmallListPartial", chats);
        }
        #endregion

        #region Форма приглашений на событие
        /// <summary>
        /// Возвращает список друзей которых можно пригласить.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task<List<InviteOutVM>> GetFriendslist(int eventId)
        {
            return await eventService.GetFriendslist(eventId, User.Identity.Name);
        }

        /// <summary>
        /// Апи получает приглашения с морды и парсит их в Invite.
        /// </summary>
        /// <param name="eventId">Ид события</param>
        /// <param name="invites">Инвайты (userId - id, message- сообщение)</param>
        /// <returns></returns>
        public async Task InviteFriendsIn(int eventId, InviteInVm[] invites)
        {
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
        public async Task<IActionResult> EventEdit(int eventId)
        {
            var curUser = await context.Users.FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);
            var eve = await context.Events.Include(e => e.EventTegs).FirstOrDefaultAsync(e => e.EventId == eventId);
            if (curUser == null || eve == null)
            {
                Response.StatusCode = 204;
                ViewBag.Status = 204;
                return View(null);
            }
            if (curUser.Id != eve.UserId)
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
                EventId = eve.EventId
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
        public async Task<IActionResult> EventEdit(EventEditVM model)
        {
            // Валидация.
            if ( string.IsNullOrWhiteSpace(model.City) || string.IsNullOrWhiteSpace(model.Tegs) ||  string.IsNullOrWhiteSpace(model.Title))
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
        public async Task<IActionResult> DeleteEvent(int eventId)
        {
            var result = await eventService.DeleteEvent(User.Identity.Name, eventId);
            if(result == 200) return RedirectToAction("Index", "MyPage");
            else return BadRequest();
        }
        #endregion
    }
}