using EventB.Hubs;
using EventB.Services.ImageService;
using EventB.Services.Logger;
using EventB.Services.MessageServices;
using EventB.ViewModels;
using EventB.ViewModels.EventsVM;
using EventB.ViewModels.MessagesVM;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
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

        private string IMAGE_SUFFIX = ".jpeg";
        private string DEFAULT_IMG_PATH = "/images/defaultimg.jpg";

        private readonly Context context;
        private readonly IMessageService messageService;
        private readonly ITegSplitter tegSplitter;
        private readonly IWebHostEnvironment environment;
        private readonly UserManager<User> userManager;
        private readonly ILogger logger;
        private readonly IImageService imageService;

        private readonly IHubContext<MessagesHub> hubContext;

        public EventService(Context _context,
            IMessageService _messageService,
            ITegSplitter _tegSplitter,
            IWebHostEnvironment _environment,
            UserManager<User> _userManager,
            ILogger _logger,
            IImageService _imageService,
            IHubContext<MessagesHub> _hubContext)
        {
            context = _context;
            messageService = _messageService;
            tegSplitter = _tegSplitter;
            environment = _environment;
            userManager = _userManager;
            logger = _logger;
            imageService = _imageService;
            hubContext = _hubContext;
        }
        /// <summary>
        /// Добавление нового события.
        /// </summary>
        /// <param name="model">Что там введено</param>
        /// <param name="userName">имя пользователя</param>
        /// <returns></returns>
        public async Task<Event> AddEvent(AddEventViewModel model, string userName)
        {
            try
            {
                await logger.LogStringToFile("Начало создания события");

                var fileName = Guid.NewGuid().ToString();
                string imgSourse = String.Concat("/images/EventImages/", fileName);
                string imgMedium = String.Concat("/images/EventImages/Medium/", "M" + fileName);
                string imgMini = String.Concat("/images/EventImages/Mini/", "m" + fileName);

                if (model.MainPicture != null)
                {
                    try
                    {
                        var newImagesDict = new Dictionary<int, string>();

                        newImagesDict.Add(0, imgSourse);
                        newImagesDict.Add(360, imgMedium);
                        newImagesDict.Add(100, imgMini);

                        newImagesDict = await imageService.SaveOriginAndResizedImagesByInputedSizes(model.MainPicture, IMAGE_SUFFIX, newImagesDict, 500);

                        imgSourse = newImagesDict[0];
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
                    await imageService.SaveResizedImage(environment.WebRootPath + DEFAULT_IMG_PATH, environment.WebRootPath + imgSourse, 800, IMAGE_SUFFIX);
                    imgSourse += IMAGE_SUFFIX;
                    await imageService.SaveResizedImage(environment.WebRootPath + DEFAULT_IMG_PATH, environment.WebRootPath + imgMedium, 360, IMAGE_SUFFIX);
                    imgMedium += IMAGE_SUFFIX;
                    await imageService.SaveResizedImage(environment.WebRootPath + DEFAULT_IMG_PATH, environment.WebRootPath + imgMini, 100, IMAGE_SUFFIX);
                    imgMini += IMAGE_SUFFIX;
                }
                
                var creator = await userManager.FindByNameAsync(userName);
                
                var vizit = new Vizit
                {
                    User = creator,
                    EventTitle = model.Title,
                    EventPhoto = imgMedium,
                    VizitorName = creator.Name,
                    VizitirPhoto = creator.MediumImage
                };

                var vizits = new List<Vizit> { vizit };

                var tegs = tegSplitter.GetEnumerable(model.Tegs.ToUpper()).ToList();
                List<EventTeg> eventTegs = new List<EventTeg>();
                foreach (var teg in tegs)
                {
                    eventTegs.Add(new EventTeg { Teg = teg });
                }
                
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
                    ChatPhoto = imgMini,
                    SystemUserName = userName
                };
                chat.UserChat.Add(userChat);
                
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
                    Type = model.Type,
                    Views = 0,
                    WillGo = 1,
                    Creator = creator,
                    Image = imgSourse,
                    CreationDate = DateTime.Now,
                    Vizits = vizits,
                    Chat = chat,
                    Phone = model.Phone,
                    AgeRestrictions = model.AgeRestrictions,
                    MediumImage = imgMedium,
                    MiniImage = imgMini
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
            catch(Exception Ex)
            {
                await logger.LogStringToFile($"Событие не сохранено по причине: {Ex.Message}: {Ex.StackTrace}");
                throw Ex;
            }            
        }

        /// <summary>
        /// Событие для деталей события.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Event> Details(int id)
        {
            // Берем юзер чат  для пользователя для отображения заблокированности
            var eve = await context.Events.
                     Include(e => e.Creator).
                     ThenInclude(e=>e.Friends).
                     Include(e => e.Chat).
                     ThenInclude(e => e.Messages).
                     Include(e => e.EventTegs).
                     Include(e => e.Vizits).
                     FirstOrDefaultAsync(e => e.EventId == id);
            eve.Views++;
            await context.SaveChangesAsync();
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

            return chat.Messages.Where(e => e.ChatId == chat.ChatId)
                .Take(100)
                .Union(context.Messages.Where(e => e.ChatId == chat.ChatId && e.EventState == true)).OrderByDescending(e => e.PostDate).ToList();
        }


        /// <summary>
        /// Отправка сообщения в чат события.
        /// </summary>
        /// <param name="userId">id пользователя</param>
        /// <param name="userName">имя пользователя</param>
        /// <param name="chatId">id чата</param>
        /// <param name="text">сообщение</param>
        /// <returns></returns>
        public async Task<int> SendMessage(string userId, User user, int chatId, string text)
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
                        ChatPhoto = chat.Event.MiniImage,
                        OpponentId = "0",
                        SystemUserName = user.UserName
                    };
                    chat.UserChat.Add(newUserChat);
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
                SenderName = user.Name,
                Text = text,
                ReciverId = "0",
                PostDate = DateTime.Now,
            };
            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();

            var dataObject = new ChatMessageVM()
            {
                chatId = chatId,
                personId = user.Id,
                postDate = message.PostDate,
                senderName = user.Name,
                text = message.Text
            };

            // Здесь отправкa сообщения через хаб всем подключенным клиентам, которые есть в чате
            foreach (var userInChat in chat.UserChat.Where(e=>e.IsDeleted == false))
            {
                await hubContext.Clients.Group(userInChat.SystemUserName).SendAsync("reciveChatMessage", dataObject);
            }

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
                Include(e=>e.Invites).
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
                // Удалить приглашение если оно есть
                var invitie = curentEv.Invites.FirstOrDefault(e => e.UserId == user.Id);
                if(invitie != null)
                {
                    context.Invites.Remove(invitie);
                }

                // Его нет. Добавить в список, добавить чат.
                if (!curentEv.Chat.UserChat.Any(e=>e.UserId == user.Id))
                {
                    var newUserChat = new UserChat
                    {
                        ChatName = curentEv.TitleShort,
                        UserId = user.Id,
                        ChatId = curentEv.Chat.ChatId,
                        ChatPhoto = curentEv.MiniImage,
                        SystemUserName = user.UserName
                    };
                    context.UserChats.Add(newUserChat);
                }
                var newVizit = new Vizit
                {
                    UserId = user.Id,
                    EventTitle = curentEv.Title,
                    EventPhoto = curentEv.MediumImage,
                    VizitorName = user.Name,
                    VizitirPhoto = user.MediumImage
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
                EventState = false,
                EventLink = eventId,
                Text = eve.Title,
                EventLinkImage = eve.MediumImage
            };

            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();

            var dataObject = new ChatMessageVM()
            {
                chatId = userChat.ChatId,
                personId = user.Id,
                postDate = message.PostDate,
                senderName = user.Name,
                text = message.Text,
                eventLink = message.EventLink,
                eventLinkImage = eve.MediumImage
            };
            var userChats = await context.UserChats.Where(e => e.IsDeleted == false && e.ChatId == userChat.ChatId).ToListAsync();
            // Здесь отправкa сообщения через хаб всем подключенным клиентам, которые есть в чате
            foreach (var userInChat in userChats)
            {
                await hubContext.Clients.Group(userInChat.SystemUserName).SendAsync("reciveChatMessage", dataObject);
            }

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
            var curUser = await context.Users.
                Include(e => e.Friends).
                FirstOrDefaultAsync(e => e.UserName == userName);
            var eve = await context.Events.Include(e => e.Invites).Include(e => e.Vizits).FirstOrDefaultAsync(e=>e.EventId == eventId);

            var friends = curUser.Friends
                .Where(e =>
                    e.IsBlocked == false &&
                    e.IsConfirmed == true &&
                    !eve.Vizits.Any(vi => e.FriendUserId == vi.UserId) &&
                    !eve.Invites.Any(inv => e.FriendUserId == inv.UserId)).
                Select(e => new InviteOutVM
                {
                    UserId = e.FriendUserId,
                    Name = e.UserName,
                    Photo = e.UserPhoto
                }).ToList();

            return friends;
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
            var eve = await context.Events.Include(e => e.Invites).Include(e => e.Vizits).FirstAsync(e => e.EventId == eventId); 
            var curUser = await context.Users.Include(e=>e.Friends).FirstOrDefaultAsync(e=>e.UserName == user);
            var newInv = new List<Invite>();

            foreach (var id in invites)
            {
                if (!curUser.Friends.Any(e => e.FriendUserId == id.userId) ||
                    eve.Invites.Any(e=>e.UserId == id.userId) ||
                    eve.Vizits.Any(e=>e.UserId == id.userId)) continue;
                
                var newItm = new Invite
                {
                    EventId = eventId,
                    UserId = id.userId,
                    InviterId = curUser.Id,
                    InviterName = curUser.Name,
                    InviterPhoto = curUser.MediumImage,
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
            var user = await userManager.FindByNameAsync(userName);
            var eve = await context.Events.Include(e => e.EventTegs)
                    .Include(e => e.Chat).ThenInclude(e => e.Messages)
                    .Include(e => e.Chat).ThenInclude(e => e.UserChat)
                    .FirstOrDefaultAsync(e => e.EventId == model.EventId);
            if (eve.UserId != user.Id) throw new Exception("Запрещено");
            List<Vizit> vizits = null;

            if (model.NewPicture != null)
            {
                var eveImgDict = new Dictionary<int, string>();
                eveImgDict.Add(0, TrimSuffix(eve.Image));
                eveImgDict.Add(360, TrimSuffix(eve.MediumImage));
                eveImgDict.Add(100, TrimSuffix(eve.MiniImage));

                await imageService.SaveOriginAndResizedImagesByInputedSizes(model.NewPicture, IMAGE_SUFFIX, eveImgDict, 500);
            }

            // Здесь подправить разметку чтобы отображать абзацами
            string messageTemplate = "Изменения в событии:<br>";
            string chatMessage = messageTemplate;

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

            if (model.Place != model.OldPlace)
            {
                eve.Place = model.Place;
                chatMessage += $"<p><span>Новое место: </span>{model.Place}</p>";
            }            

            if (model.Date.HasValue && model.Date != model.OldDate) // Это вернуть когда прийдет время && eve.Type == EventType.Private)
            {
                eve.Date = model.Date.Value;
                chatMessage += $"<p><span>Новое время: </span>{model.Date.Value.ToString("dd.MM.yy HH:mm")}</p>";
            }

            if (model.City != model.OldCity)
            {
                eve.City = model.City;
                eve.NormalizedCity = model.City.ToUpper();
                chatMessage += $"<p><span>Новый город: </span>{model.City}</p>";
            }

            if (model.Tegs != eve.Tegs)
            {
                var tegs = tegSplitter.GetEnumerable(model.Tegs.ToUpper()).ToList();
                List<EventTeg> eventTegs = new List<EventTeg>();
                foreach (var teg in tegs)
                {
                    eventTegs.Add(new EventTeg { Teg = teg });
                }
                eve.EventTegs = eventTegs;
                chatMessage += $"<p><span>Новые теги: </span>{("@" + string.Join(" @", eventTegs.Select(e => e.Teg).ToList()))}</p>";
            }

            if (model.OldPhone != model.Phone)
            {
                eve.Phone = model.Phone;
                chatMessage += $"<p><span>Новый контактный телефон: </span>{model.Phone}</p>";
            }

            if (model.Body != model.OldBody)
            {
                eve.Body = model.Body;
                chatMessage += $"<p>Изменено описание.</p>";
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

            eve.AgeRestrictions = model.AgeRest;
            
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

            context.Events.Update(eve);
            await context.SaveChangesAsync();

            var dataObject = new ChatMessageVM()
            {
                chatId = eve.Chat.ChatId,
                personId = user.Id,
                postDate = DateTime.Now,
                senderName = user.Name,
                text = chatMessage,
                eventState = true
            };
            var userChats = await context.UserChats.Where(e => e.IsDeleted == false && e.ChatId == eve.Chat.ChatId).ToListAsync();
            // Здесь отправкa сообщения через хаб всем подключенным клиентам, которые есть в чате
            foreach (var userInChat in userChats)
            {
                await hubContext.Clients.Group(userInChat.SystemUserName).SendAsync("reciveChatMessage", dataObject);
            }

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
                return 401;
            }

            await imageService.DeleteImage(environment.WebRootPath + eve.Image);
            await imageService.DeleteImage(environment.WebRootPath + eve.MediumImage);
            await imageService .DeleteImage(environment.WebRootPath + eve.MiniImage);

            context.Remove(eve);

            await context.SaveChangesAsync();
            return 200;

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
