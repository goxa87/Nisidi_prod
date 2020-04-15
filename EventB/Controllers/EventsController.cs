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

namespace EventB.Controllers
{
    public class EventsController : Controller
    {
        private Context context { get; }
        private readonly SignInManager<User> userManager;
        readonly IUserFindService userFindService;
        IWebHostEnvironment environment;
        ITegSplitter tegSplitter;
        IEventSelectorService eventSelector;

        public EventsController(Context c,
            SignInManager<User> UM,
            IWebHostEnvironment env,
            IUserFindService _userFindService,
            ITegSplitter _tegSplitter,
            IEventSelectorService _eventSelector)
        {
            context = c;
            userManager = UM;
            environment = env;
            userFindService = _userFindService;
            tegSplitter = _tegSplitter;
            eventSelector = _eventSelector;
        }

        public async Task<IActionResult> Start()
        {
            List<Event> events;
            if (User.Identity.IsAuthenticated)
            {
                var user = await userFindService.GetCurrentUserAsync(User.Identity.Name);
                events = await eventSelector.GetStartEventListAsync(user);
                // параллельно обновляем количество просмотров в другом потоке.
                Task.Factory.StartNew(async () =>
                {
                    foreach (var ev in events)
                    {
                        ev.Views++;
                    }

                    context.UpdateRange(events);
                    await context.SaveChangesAsync();
                });
                return View(events);
            }
            else 
            {
                events = await eventSelector.GetStartEventListAsync(null);
            }

            return View(events);
        }

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
                // Значение картинки если ее нет.
                string src = "/images/defaultimg.jpg";
                if (model.MainPicture != null)
                {
                    // Формирование строки имени картинки.
                    string fileName = String.Concat(DateTime.Now.ToString("dd-MM-yy_hh-mm"), "_", model.MainPicture.FileName);
                    src = String.Concat("/images/EventImages/", fileName);
                    // Запись на диск.
                    using (var FS = new FileStream(environment.WebRootPath + src, FileMode.Create))
                    {
                        await model.MainPicture.CopyToAsync(FS);
                    }
                }
                // Пользователь который выложил.
                var creator = await context.Users.FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);
                // Добавляем в посетителей автора.
                var vizit = new Vizit
                {
                    User = creator,
                    EventTitle = model.Title,
                    EventPhoto = src,
                    VizitorName = creator.Name,
                    VizitirPhoto = "/images/defaultimg.jpg"
                };

                var vizits = new List<Vizit> { vizit };

                var tegs = tegSplitter.GetEnumerable(model.Tegs).ToList();
                List<EventTeg> eventTegs = new List<EventTeg>();
                foreach (var teg in tegs)
                {
                    eventTegs.Add(new EventTeg { Teg = teg });
                }
                // Итоговое формирование события.
                var eve = new Event
                {
                    Title = model.Title,
                    Body = model.Body,
                    EventTegs = eventTegs,
                    City = model.City,
                    Place = model.Place,
                    Date = model.Date,
                    Type = EventType.Private,
                    Likes = 0,
                    Views = 0,
                    Shares = 0,
                    WillGo = 1,
                    Creator = creator,
                    Image = src,
                    CreationDate = DateTime.Now,
                    Vizits = vizits
                };

                await context.Events.AddAsync(eve);
                await context.SaveChangesAsync();
                return RedirectToAction("Start");
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
            if (id != null)
            {
                var eve = await context.Events.
                    Include(e => e.Creator).
                    Include(e=>e.Chat).
                    Include(e=>e.EventTegs).
                    Include(e => e.Vizits).
                    FirstOrDefaultAsync(e => e.EventId == id);

                Chat chat;

                if (eve.Chat!=null)
                {
                    var messages = await context.Messages.
                        Where(e => e.ChatId == eve.Chat.ChatId).
                        OrderByDescending(e => e.PostDate).
                        Take(30).ToListAsync();

                    chat = new Chat
                    {
                        ChatId = eve.Chat.ChatId,
                        Messages = messages
                    };
                }
                else
                {
                    // Создает псевдо класс который нигде не сохраняется.(для отображения в вью).
                    chat = new Chat
                    {
                        ChatId = 0,
                        Messages = new List<Message>()
                    };
                }
                eve.Chat = chat;
                var user = await userFindService.GetCurrentUserAsync(User.Identity.Name);
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
        // Создание чата
        /// <summary>
        /// Создание чата для события.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("/Events/CreateEventChat")]
        [Authorize]
        public async Task<int> CreateEventChat(int eventId, string userId)
        {
            // Здесь может быть логика оповещения владельца события о начале чата.
            // Создание чата для первого пользователя.
            var event1 = await context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            if (event1 == null)
            {
                Response.StatusCode = 204;
                return 0;
            }
            var name = event1.Title.Length > 50 ? event1.Title.Remove(49) : event1.Title;

            var userChat = new UserChat
            {
                UserId = userId,
                ChatName = name
            };
            var chat = new Chat
            {
                UserChat = new List<UserChat>
                {
                    userChat
                },
                EventId = eventId
            };
            event1.Chat = chat;
            context.Events.Update(event1);
            await context.SaveChangesAsync();

            return chat.ChatId;
        }
        /// <summary>
        /// Отправка сообщения в чат события.
        /// </summary>
        /// <param name="userId">id пользователя</param>
        /// <param name="chatId">id чата</param>
        /// <returns></returns>
        [Authorize]
        [Route("/Events/SendMessage")]
        public async Task<StatusCodeResult> SendMessage(string userId,string userName, int chatId, string text)
        {
            var chat = await context.Chats.Include(e=>e.Event).
                FirstOrDefaultAsync(e => e.ChatId == chatId);
            if (chat == null)
            {
                Response.StatusCode = 204;
                return StatusCode(204);
            }
            // Создаем UserChat для отображения в списке чатов.

            //var task1 = Task.Run(async () =>
            //{
            //    var userChat = await context.UserChats.
            //    FirstOrDefaultAsync(e => e.UserId == userId && e.ChatId == chatId);
            //    if (userChat == null)
            //    {
            //        Thread.Sleep(2000);
            //        // Создать юзер чат.
            //        var newUserChat = new UserChat
            //        {
            //            ChatName = chat.Event.TitleShort + "...",
            //            UserId = userId,
            //            ChatId = chatId
            //        };
            //        await context.UserChats.AddAsync(newUserChat);
            //        await context.SaveChangesAsync();
            //    }
            //});

            var userChat = await context.UserChats.
                FirstOrDefaultAsync(e => e.UserId == userId && e.ChatId == chatId);
            if (userChat == null)
            {
                // Создать юзер чат.
                var newUserChat = new UserChat
                {
                    ChatName = chat.Event.TitleShort,
                    UserId = userId,
                    ChatId = chatId
                };
                await context.UserChats.AddAsync(newUserChat);
                await context.SaveChangesAsync();
            }

            Message message = new Message
            {
                ChatId= chatId,
                PersonId = userId,
                SenderName = userName,
                Text = text,
                ReciverId = "0",
                PostDate = DateTime.Now,
                Read = false
            };            
            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();
            //task1.Wait();
            return Ok();            
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
    }
}