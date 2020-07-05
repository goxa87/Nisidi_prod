﻿using System;
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
            args.DateDue = args.DateDue.AddDays(1);
            args.Title = args.Title ?? "";
            args.City = args.City ?? "";
            args.Tegs = args.Tegs ?? "";
            var rezult = await eventSelector.GetCostomEventsAsync(args);
            // Пропускаем то что уж нашли.
            args.Skip += args.Take;
            var VM = new EventListVM
            {
                events = rezult,
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








            // Проверить подписан ли 
            // Удалить визит
            // Или создать визит и добавить чат
            var user = await userFindService.GetCurrentUserAsync(User.Identity.Name);

            var curentEv = await context.Events.
                Include(e => e.Vizits).ThenInclude(e => e.User).
                Include(e => e.Chat).ThenInclude(e => e.UserChat).
                FirstOrDefaultAsync(e => e.EventId == eventId);
            // Если не найдено событие.
            if (curentEv == null)
            {
                return StatusCode(204);
            }
            // Выбираем визит к событию. 
            var vizit = curentEv.Vizits.FirstOrDefault(e => e.UserId == user.Id);
            if (vizit != null)
            {
                // он есть как визитор. Удалить. Удалить чат.
                curentEv.Vizits.Remove(vizit);

                var userChat = curentEv.Chat.UserChat.FirstOrDefault(e => e.UserId == user.Id);
                if (userChat != null)
                {
                    curentEv.Chat.UserChat.Remove(userChat);
                }
                curentEv.WillGo--;
            }
            else
            {
                // Его нет. Добавить в список, добавить чат.
                var newVizit = new Vizit
                {
                    UserId = user.Id,
                    EventTitle = curentEv.TitleShort,
                    EventPhoto = curentEv.Image,
                    VizitorName = user.Name,
                    VizitirPhoto = user.Photo
                };
                curentEv.Vizits.Add(newVizit);
                curentEv.WillGo++;
                context.Events.Update(curentEv);

                var newUserChat = new UserChat
                {
                    ChatName = curentEv.TitleShort,
                    UserId = user.Id,
                    ChatId = curentEv.Chat.ChatId
                };
                await context.UserChats.AddAsync(newUserChat);

            }
            await context.SaveChangesAsync();
            return Ok();
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
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var userChat = await context.UserChats.Include(e => e.Chat).FirstOrDefaultAsync(e => e.UserChatId == userChatId);
            if (userChat == null) return BadRequest();
            var eve = await context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            if (eve == null) return BadRequest();
            var message = new Message
            {
                PersonId = user.Id,
                SenderName = user.Name,
                ChatId = userChat.ChatId,
                PostDate = DateTime.Now,
                Read = false,
                EventState = false,
                EventLink = eventId,
                Text = eve.Title,
                EventLinkImage = eve.Image
            };

            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();
            return Ok();
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

        /// <summary>
        /// Получение новых сообщений динамически.
        /// </summary>
        /// <param name="chatId">ИД чата</param>
        /// <param name="lastMes"> доля секунды последнего получения сообщения</param>
        /// <returns></returns>        
        [Authorize]
        [Route("GetNewMessage")]
        public async Task GetNewMessage(int chatId, int lastMes)
        {

        }

        [Authorize]
        [Route("GetHistory")]
        public async Task GetHistory(int chatId, int firstMessage, int Count = 30)
        {

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
            var curUser = await context.Users.
                FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);

            var friends = await context.Friends.Where(e => e.FriendUserId == curUser.Id
                && e.IsBlocked == false
                && e.IsConfirmed == true).
                Select(e => new InviteOutVM
                {
                    UserId = e.UserId,
                    Name = e.UserName,
                    Photo = e.UserPhoto
                }).ToListAsync();
            // Вычесть тех , которые уже пойдут на событие. 
            var willGoId = context.Vizits.Where(e => e.EventId == eventId).Select(e => e.UserId);
            var allId = context.Users.Select(e => e.Id);
            var willWillNot = await allId.Except(willGoId).ToListAsync();
            var rezult = friends.Join(willWillNot, f => f.UserId, w => w, (f, w) => f).ToList();
            return rezult;
        }
        /// <summary>
        /// Апи получает приглашения с морды и парсит их в Invite.
        /// </summary>
        /// <param name="eventId">Ид события</param>
        /// <param name="invites">Инвайты (userId - id, message- сообщение)</param>
        /// <returns></returns>
        public async Task InviteFriendsIn(int eventId, InviteInVm[] invites)
        {
            var curUser = await userFindService.GetCurrentUserAsync(User.Identity.Name);
            var newInv = new List<Invite>();

            foreach (var id in invites)
            {
                var newItm = new Invite
                {
                    EventId = eventId,
                    UserId = id.userId,
                    InviterId = curUser.Id,
                    InviterName = curUser.Name,
                    InviterPhoto = curUser.Photo,
                    InviteDescription = id.message
                };
                newInv.Add(newItm);
            }

            await context.Invites.AddRangeAsync(newInv);
            await context.SaveChangesAsync();
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
                var eve = await context.Events.Include(e => e.EventTegs)
                    .Include(e=>e.Chat).ThenInclude(e=>e.Messages)
                    .Include(e=>e.Chat).ThenInclude(e=>e.UserChat)
                    .FirstOrDefaultAsync(e => e.EventId == model.EventId);
                List<Vizit> vizits = null;
                // Значение картинки если ее нет.
                if (model.NewPicture != null)
                {
                    string src = "/images/defaultimg.jpg";
                    // Формирование строки имени картинки.
                    string fileName = String.Concat(DateTime.Now.ToString("dd-MM-yy_hh-mm"), "_", model.NewPicture.FileName);
                    src = String.Concat("/images/EventImages/", fileName);
                    // Запись на диск.
                    using (var FS = new FileStream(environment.WebRootPath + src, FileMode.Create))
                    {
                        await model.NewPicture.CopyToAsync(FS);
                    }
                    vizits = await context.Vizits.Where(e => e.EventId == eve.EventId).ToListAsync();
                    foreach (var e in vizits)
                    {
                        e.EventPhoto = src;
                    }
                    eve.Image = src;
                    context.Vizits.UpdateRange(vizits);
                }
                // Здесь подправить разметку чтобы отображать абзацами
                string chatMessage = "Изменения в событии:<br>";
                // При внесении изменений в теги.
                if (model.Tegs != eve.Tegs)
                {
                    var tegs = tegSplitter.GetEnumerable(model.Tegs.ToUpper()).ToList();
                    List<EventTeg> eventTegs = new List<EventTeg>();
                    foreach (var teg in tegs)
                    {
                        eventTegs.Add(new EventTeg { Teg = teg });
                    }
                    eve.EventTegs = eventTegs;
                    chatMessage += $"<p>Новые теги</p><p>{model.Tegs}</p><br>";
                }
                // При внесении изменений в назавание.
                if(model.Title != model.OldTitle)
                {
                    eve.Title = model.Title;
                    eve.NormalizedTitle = model.Title.ToUpper();
                    foreach (var e in eve.Chat.UserChat)
                    {
                        e.ChatName = eve.TitleShort;
                    }
                    vizits = vizits != null ? vizits : await context.Vizits.Where(e => e.EventId == eve.EventId).ToListAsync();
                    foreach (var e in vizits)
                    {
                        e.EventTitle = eve.TitleShort;
                    }                    
                    context.Vizits.UpdateRange(vizits);
                    chatMessage += $"<p>Новое название</p><p>{model.Title}</p><br>";
                }
                if (model.Body != model.OldBody)
                {
                    eve.Body = model.Body;
                    chatMessage += $"<p>Изменено описание.</p><br>";
                }
                if (model.Place != model.OldPlace)
                {
                    eve.Place = model.Place;
                    chatMessage += $"<p>Новое место</p><p>{model.Place}</p><br>";
                }
                if (model.City != model.OldCity)
                {
                    eve.City = model.City;
                    eve.NormalizedCity = model.City.ToUpper();
                    chatMessage += $"<p>Новый город</p><p>Стало: {model.City}</p><br>";
                }
                if (model.Date != model.OldDate && eve.Type == EventType.Private)
                {
                    eve.Date = model.Date;
                    chatMessage += $"<p>новое время</p><p>Стало: {model.Date.ToString("dd.MM.yy hh:mm")}</p><br>";
                }
                var user = await userManager.GetUserAsync(User);
                if(chatMessage != "<br>")
                {
                    eve.Chat.Messages.Add(new Message
                    {
                        PersonId = user.Id,
                        EventState = true,
                        PostDate = DateTime.Now,
                        SenderName = user.Name,
                        Text = chatMessage
                    });                    
                }
                context.Events.Update(eve);
                await context.SaveChangesAsync();
                return Redirect($"/Events/Details/{eve.EventId}");
            }        
        }
        [Authorize]
        public async Task<IActionResult> DeleteEvent(int eventId)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var eve = await context.Events.Include(e => e.Chat).ThenInclude(e => e.UserChat)
                .Include(e => e.Chat).ThenInclude(e => e.Messages)
                .Include(e => e.EventTegs)
                .Include(e => e.Vizits).FirstOrDefaultAsync(e => e.EventId == eventId);

            if(user.Id != eve.UserId)
            {
                return BadRequest();
            }

            context.Remove(eve);
            await context.SaveChangesAsync();
            return RedirectToAction("Index", "MyPage");
        }
        #endregion
    }
}