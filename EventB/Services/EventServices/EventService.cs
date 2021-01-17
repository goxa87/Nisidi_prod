using EventB.Services.Logger;
using EventB.Services.MessageServices;
using EventB.ViewModels;
using EventB.ViewModels.EventsVM;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Schema;
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
        private readonly ILogger logger;

        public EventService(Context _context,
            IMessageService _messageService,
            ITegSplitter _tegSplitter,
            IWebHostEnvironment _environment,
            UserManager<User> _userManager,
            ILogger _logger
        )
        {
            context = _context;
            messageService = _messageService;
            tegSplitter = _tegSplitter;
            environment = _environment;
            userManager = _userManager;
            logger = _logger;
        }
        /// <summary>
        /// Добавление нового события.
        /// </summary>
        /// <param name="model">Что там введено</param>
        /// <param name="userName">имя пользователя</param>
        /// <returns></returns>
        public async Task<Event> AddEvent(AddEventViewModel model, string userName)
        {
            await logger.LogStringToFile("Начало создания события");
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
                Views = 0,
                WillGo = 1,
                Creator = creator,
                Image = src,
                CreationDate = DateTime.Now,
                Vizits = vizits,
                Chat = chat,
                Phone = model.Phone,
                AgeRestrictions = model.AgeRestrictions
            };

            if (!string.IsNullOrWhiteSpace(model.TicketsDesc))
            {
                eve.TicketsDesc = model.TicketsDesc;
                eve.Tickets = true;
            }

            await context.Events.AddAsync(eve);
            await context.SaveChangesAsync();
            await logger.LogObjectToFile("Создано событие ", eve);
            await logger.LogStringToFile($"Окончание создания события с номером {eve.EventId}");
            return eve;
        }

        /// <summary>
        /// Событие для деталей события.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Event> Details(int id, string userId = null )
        {
            // Берем юзер чат  для пользователя для отображения заблокированности
            var eve = await context.Events.
                     Include(e => e.Creator).
                     Include(e => e.Chat).
                     ThenInclude(e => e.Messages).
                     Include(e => e.EventTegs).
                     Include(e => e.Vizits).
                     FirstOrDefaultAsync(e => e.EventId == id);
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var userChat = await context.UserChats.Where(e => e.ChatId == eve.Chat.ChatId && e.UserId == userId).ToListAsync();
                eve.Chat.UserChat = userChat;
            }
            else
            {
                eve.Chat.UserChat = new List<UserChat>(0);
            }
            return eve;
        }

        /// <summary>
        /// Список изменений в событии.
        /// </summary>
        /// <param name="EventId">Id События</param>
        /// <returns></returns>
        public async Task<List<Message>> GetEventMessages(int EventId)
        {
            var chat = await context.Chats.Include(e => e.Messages).Where(e => e.EventId == EventId).FirstOrDefaultAsync();

            return await context.Messages.Where(e => e.ChatId == chat.ChatId)
                .Take(40)
                .Union(context.Messages.Where(e => e.ChatId == chat.ChatId && e.EventState == true)).OrderByDescending(e => e.PostDate).ToListAsync();
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
            var chat = await context.Chats.Include(e => e.Event)
                .Include(e=>e.UserChat)
                .FirstOrDefaultAsync(e => e.ChatId == chatId);
            if (chat == null)
            {
                return 204;
            }

            var userChat = chat.UserChat.
                FirstOrDefault(e => e.UserId == userId);
            if (userChat != null && userChat.IsBlockedInChat)
            {
                return 403;
            }
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
            else
            {
                if (userChat.IsDeleted)
                {
                    userChat.IsDeleted = false;
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
            return 200;
        }

        /// <summary>
        /// Подписаться на событие.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task<int> SubmitToEvent(int eventId, string name)
        {
            // Проверить подписан ли 
            // Удалить визит
            // Или создать визит и добавить чат
            var user = await userManager.FindByNameAsync(name);

            var curentEv = await context.Events.
                Include(e => e.Vizits).ThenInclude(e => e.User).
                Include(e => e.Chat).ThenInclude(e => e.UserChat).
                FirstOrDefaultAsync(e => e.EventId == eventId);
            // Если не найдено событие.
            if (curentEv == null)
            {
                return 204;
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
                if(!curentEv.Chat.UserChat.Any(e=>e.UserId == user.Id))
                {
                    var newUserChat = new UserChat
                    {
                        ChatName = curentEv.TitleShort,
                        UserId = user.Id,
                        ChatId = curentEv.Chat.ChatId,
                        ChatPhoto = curentEv.Image
                    };
                    await context.UserChats.AddAsync(newUserChat);
                }
                var newVizit = new Vizit
                {
                    UserId = user.Id,
                    EventTitle = curentEv.Title,
                    EventPhoto = curentEv.Image,
                    VizitorName = user.Name,
                    VizitirPhoto = user.Photo
                };
                curentEv.Vizits.Add(newVizit);
                curentEv.WillGo++;
                context.Events.Update(curentEv);
            }
            await context.SaveChangesAsync();
            return 200;
        }

        /// <summary>
        /// Отправка сообщения в чат
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="userChatId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<int> SendLink(int eventId, int userChatId, string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            var userChat = await context.UserChats.Include(e => e.Chat).FirstOrDefaultAsync(e => e.UserChatId == userChatId);
            if (userChat == null) return 400;
            var eve = await context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            if (eve == null) return 400;
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
            return 200;
        }

        /// <summary>
        /// Получение списка друзей, которых можно пригласить на событие.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<List<InviteOutVM>> GetFriendslist(int eventId, string userName)
        {
            var curUser = await userManager.FindByNameAsync(userName);
            // не оздавать приглашения для тех, кто уже пойдет или имеет приглашение

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
            var alreadyHaveinvite = context.Invites.Where(e => e.EventId == eventId).Select(e => e.UserId);
            var allId = context.Users.Select(e => e.Id);
            var willWillNot = await allId.Except(willGoId).Except(alreadyHaveinvite).ToListAsync();
            var rezult = friends.Join(willWillNot, f => f.UserId, w => w, (f, w) => f).ToList();
            return rezult;
        }

        /// <summary>
        /// Приглашение Друзей на событие
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="user"></param>
        /// <param name="invites"></param>
        /// <returns></returns>
        public async Task InviteFriendsIn(int eventId, string user, InviteInVm[] invites)
        {
            var curUser = await userManager.FindByNameAsync(user);
            var newInv = new List<Invite>();

            foreach (var id in invites)
            {
                // не оздавать приглашения для тех, кто уже пойдет или имеет приглашение
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


        /// <summary>
        /// Редактирование события
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Event> EventEdit(string userName, EventEditVM model)
        {
            var eve = await context.Events.Include(e => e.EventTegs)
                    .Include(e => e.Chat).ThenInclude(e => e.Messages)
                    .Include(e => e.Chat).ThenInclude(e => e.UserChat)
                    .FirstOrDefaultAsync(e => e.EventId == model.EventId);
            List<Vizit> vizits = null;
            // Значение картинки если ее нет.
            if (model.NewPicture != null)
            {
                string src = "/images/defaultimg.jpg";
                // Формирование строки имени картинки.
                string fileName = String.Concat(DateTime.Now.ToString("dd-MM-yy_HH-mm"), "_", model.NewPicture.FileName);
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
            string messageTemplate = "Изменения в событии:<br>";
            string chatMessage = messageTemplate;
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
                chatMessage += $"<p><span>Новые теги: </span>{model.Tegs}</p>";
            }
            // При внесении изменений в назавание.
            if (model.Title != model.OldTitle)
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
                chatMessage += $"<p><span>Новое название: </span>{model.Title}</p>";
            }
            if (model.Body != model.OldBody)
            {
                eve.Body = model.Body;
                chatMessage += $"<p>Изменено описание.</p>";
            }
            if (model.Place != model.OldPlace)
            {
                eve.Place = model.Place;
                chatMessage += $"<p><span>Новое место: </span>{model.Place}</p>";
            }
            if(model.Tickets != model.OldTickets)
            {
                if (!string.IsNullOrWhiteSpace(model.Tickets))
                {
                    eve.TicketsDesc = model.Tickets;
                    chatMessage += $"<p><span>Билеты: </span>{model.Tickets}</p>";
                    eve.Tickets = true;
                }
                else
                {
                    eve.TicketsDesc = model.Tickets;
                    eve.Tickets = false;
                }
            }
            if (model.City != model.OldCity)
            {
                eve.City = model.City;
                eve.NormalizedCity = model.City.ToUpper();
                chatMessage += $"<p><span>Новый город: </span>{model.City}</p>";
            }
            if (model.Date != model.OldDate && eve.Type == EventType.Private)
            {
                eve.Date = model.Date;
                chatMessage += $"<p><span>Новое время: </span>{model.Date.ToString("dd.MM.yy HH:mm")}</p>";
            }
            var user = await userManager.FindByNameAsync(userName);
            if (chatMessage != messageTemplate)
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
            eve.Phone = model.Phone;
            eve.AgeRestrictions = model.AgeRest;
            context.Events.Update(eve);
            await context.SaveChangesAsync();
            return eve;

        }

        /// <summary>
        /// Удаление события.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task<int> DeleteEvent(string username, int eventId)
        {
            var user = await userManager.FindByNameAsync(username);
            var eve = await context.Events.Include(e => e.Chat).ThenInclude(e => e.UserChat)
                .Include(e => e.Chat).ThenInclude(e => e.Messages)
                .Include(e => e.EventTegs)
                .Include(e => e.Vizits).FirstOrDefaultAsync(e => e.EventId == eventId);

            if (user.Id != eve.UserId)
            {
                return 400;
            }

            context.Remove(eve);
            await context.SaveChangesAsync();
            return 200;

        }
    }
}
