using EventB.Services.MessageServices;
using EventB.ViewModels;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.EventServices
{
    /// <summary>
    /// Сервис для Event контроллера
    /// </summary>
    public class EventService : IEventService
    {
        private readonly Context context;
        private readonly IMessageService messageService;
        private readonly ITegSplitter tegSplitter;
        private readonly IWebHostEnvironment environment;
        private readonly UserManager<User> userManager;


        public EventService(Context _context,
            IMessageService _messageService,
            ITegSplitter _tegSplitter,
            IWebHostEnvironment _environment,
            UserManager<User> _userManager
        )
        {
            context = _context;
            messageService = _messageService;
            tegSplitter = _tegSplitter;
            environment = _environment;
            userManager = _userManager;
        }
        /// <summary>
        /// Добавление нового события.
        /// </summary>
        /// <param name="model">Что там введено</param>
        /// <param name="userName">имя пользователя</param>
        /// <returns></returns>
        public async Task<Event> AddEvent(AddEventViewModel model, string userName)
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
            var creator = await userManager.FindByNameAsync(userName);
            // Добавляем в посетителей автора.
            var vizit = new Vizit
            {
                User = creator,
                EventTitle = model.Title,
                EventPhoto = src,
                VizitorName = creator.Name,
                VizitirPhoto = creator.Photo
            };

            var vizits = new List<Vizit> { vizit };

            var tegs = tegSplitter.GetEnumerable(model.Tegs.ToUpper()).ToList();
            List<EventTeg> eventTegs = new List<EventTeg>();
            foreach (var teg in tegs)
            {
                eventTegs.Add(new EventTeg { Teg = teg });
            }
            // Создание чата к событию.
            var chat = new Chat
            {
                Messages = new List<Message>
                    {
                        new Message
                        {
                            PersonId = creator.Id,
                            PostDate = DateTime.Now,
                            SenderName = creator.Name,
                            Text = "Событие создано!",
                            EventState = true
                        }
                    },
                UserChat = new List<UserChat>()
            };

            UserChat userChat = new UserChat
            {
                UserId = creator.Id,
                ChatName = model.Title.Length > 25 ? model.Title.Remove(25) + "..." : model.Title,
                ChatPhoto = src
            };
            chat.UserChat.Add(userChat);
            // Итоговое формирование события.
            var eve = new Event
            {
                Title = model.Title,
                NormalizedTitle = model.Title.ToUpper(),
                Body = model.Body,
                EventTegs = eventTegs,
                City = model.City,
                NormalizedCity = model.City.ToUpper(),
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
                Vizits = vizits,
                Chat = chat
            };

            await context.Events.AddAsync(eve);
            await context.SaveChangesAsync();

            return eve;
        }

        /// <summary>
        /// Событие для деталей события.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Event> Details(int id)
        {
            return( await context.Events.
                     Include(e => e.Creator).
                     Include(e => e.Chat).
                     Include(e => e.EventTegs).
                     Include(e => e.Vizits).
                     FirstOrDefaultAsync(e => e.EventId == id));
        }

        /// <summary>
        /// Список изменений в событии.
        /// </summary>
        /// <param name="EventId">Id События</param>
        /// <returns></returns>
        public async Task<List<Message>> GetEventMessages(int EventId)
        {
            var changes = await messageService.GetEventChangesMessages(EventId);
            var chtMess = await messageService.GetEventChatMessages(EventId);


            return changes.Union(chtMess).OrderByDescending(e=>e.PostDate).ToList();
        }


        /// <summary>
        /// Отправка сообщения в чат события.
        /// </summary>
        /// <param name="userId">id пользователя</param>
        /// <param name="userName">имя пользователя</param>
        /// <param name="chatId">id чата</param>
        /// <param name="text">сообщение</param>
        /// <returns></returns>
        public async Task<int> SendMessage(string userId, string userName, int chatId, string text)
        {
            var chat = await context.Chats.Include(e => e.Event).
                FirstOrDefaultAsync(e => e.ChatId == chatId);
            if (chat == null)
            {
                return 204;
            }

            var userChat = await context.UserChats.
                FirstOrDefaultAsync(e => e.UserId == userId && e.ChatId == chatId);

            if (userChat == null)
            {
                try
                {
                    var newUserChat = new UserChat
                    {
                        ChatName = chat.Event.TitleShort,
                        UserId = userId,
                        ChatId = chatId,
                        ChatPhoto = chat.Event.Image,
                        OpponentId = "0"                        
                    };
                    await context.UserChats.AddAsync(newUserChat);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Debug.Write("Создание чата для события", ex.ToString());
                }                
            }

            Message message = new Message
            {
                ChatId = chatId,
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
            return 200;
        }
    }
}
