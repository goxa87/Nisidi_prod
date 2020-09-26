using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBLib.DataContext;
using EventBLib.Models;
using EventB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventB.ViewModels.MessagesVM;
using Microsoft.AspNetCore.Identity;
using EventB.ViewModels;

namespace EventB.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly ITegSplitter tegSplitter;
        readonly Context context;
        readonly IUserFindService userFind;
        readonly UserManager<User> userManager;
        public ApiController(
            Context _context,
            UserManager<User> _userManager,
            ITegSplitter _tegSplitter, 
            IUserFindService _userFind
            )
        {
            tegSplitter = _tegSplitter;
            context = _context;
            userManager = _userManager;
            userFind = _userFind;
        }

        /// <summary>
        /// Вернет обновления для меню
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("get-updates-for-menu")]
        public async Task<MenuUpdatesApiVM> GetUpdatesForMenu()
        {
            var user = await context.Users.Include(e => e.Invites).Include(e => e.Friends).Include(e => e.UserChats).ThenInclude(e => e.Chat).ThenInclude(e => e.Messages).FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);
            var friends = await context.Friends.Where(e => e.FriendUserId == user.Id).ToListAsync();
            return new MenuUpdatesApiVM()
            {
                HasResult = true,
                HasNewFriends = friends.Any(e => e.FriendInitiator == false && e.IsConfirmed == false),
                HasNewInvites = user.Invites.Any(),
                HasNewMessages = user.UserChats.Any(e=>e.Chat.Messages.Any(y=>y.Read == false && y.PersonId != user.Id))                
            };
        }

        #region Секция ПОДПИСКИ
        /// <summary>
        /// Добавление Пользователя в друзья.
        /// </summary>
        /// <param name="userId">ID пользователя, который должен быть добавлен в друзья.</param>
        /// <returns></returns>
        [Route("AddFriend")]
        [Authorize]
        public async Task<StatusCodeResult> AddAsFriend(string userId)
        {           
            // Здесь появились новые поял для класса Friend/ добавить инициализацию.
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var currentUser = await userFind.GetCurrentUserAsync(User.Identity.Name);
                var friend = await userFind.GetUserByIdAsync(userId);
                // Если есть такая запись о друзьях уже есть просто возвратить статус.
                if (context.Friends.Where(e => e.UserId == friend.Id && e.FriendUserId == currentUser.Id).Any())
                {
                    return StatusCode(204);
                }
                // Создание новых записей (прямой и обратный друг). 
                var userFriend = new Friend
                {
                    UserId = friend.Id,
                    FriendUserId = currentUser.Id,
                    UserName = friend.Name,
                    UserPhoto = friend.Photo,
                    FriendInitiator=true
                };
                var userFriendReverce = new Friend
                {
                    UserId = currentUser.Id,
                    FriendUserId = friend.Id,
                    UserName = currentUser.Name,
                    UserPhoto = currentUser.Photo
                };

                await context.Friends.AddAsync(userFriend);
                await context.Friends.AddAsync(userFriendReverce);
                await context.SaveChangesAsync();

                return StatusCode(200);
            }
            else
            {
                return StatusCode(404);
            }           
        }
        /// <summary>
        /// Подтверждение для друга.
        /// </summary>
        /// <param name="friendEntityId">id записи в бд</param>
        /// <returns></returns>
        [Route("SubmitFriend")]
        [Authorize]
        public async Task<StatusCodeResult> SubmitFriend(string friendId)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var entity = await context.Friends.FirstOrDefaultAsync(e => e.UserId == friendId && e.FriendUserId == user.Id);
            var entityOpponent =  await context.Friends.FirstOrDefaultAsync(e => e.UserId == user.Id && e.FriendUserId == friendId);
            if (entity == null)
            {
                return StatusCode(204);
            }
            entity.IsConfirmed = true;
            entityOpponent.IsConfirmed = true;
            context.Friends.Update(entity);
            context.Friends.Update(entityOpponent);
            await context.SaveChangesAsync();
            return StatusCode(200);
        }
        /// <summary>
        /// Блокировать пользователя.
        /// </summary>
        /// <param name="curentUserId">текущий ползователь , который блокирует.</param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        [Authorize]
        [Route("BlockUser")]
        public async Task<StatusCodeResult> BlockUser( string friendId)
        {
            // Валидация.
            if (string.IsNullOrWhiteSpace(friendId))
            {
                return StatusCode(400);
            }
            if(!context.Users.Any(e=>e.Id==friendId))
            {
                return StatusCode(400);
            }
            // Друг и обратный друг.    
            var currentUser = await userManager.FindByNameAsync(User.Identity.Name);
            var friend = await context.Friends.FirstOrDefaultAsync(e => e.FriendUserId == currentUser.Id && e.UserId == friendId);
            var opponentFriend = await context.Friends.FirstOrDefaultAsync(e=>e.FriendUserId == friendId && e.UserId == currentUser.Id);
            // Если не инициатор, то запрещаем.
            if (friend != null && (!friend.BlockInitiator && friend.IsBlocked ))
            {
                return StatusCode(205);
            }

            friend.IsBlocked = friend.IsBlocked ? false : true;
            friend.BlockInitiator = friend.BlockInitiator ? false : true;
            opponentFriend.IsBlocked = opponentFriend.IsBlocked ? false : true;
           
            context.Update(friend);
            context.Update(opponentFriend);
            await context.SaveChangesAsync();

            return StatusCode(200);
        }

        /// <summary>
        /// Удаление друга.
        /// </summary>
        /// <param name="friendId"></param>
        /// <returns></returns>
        [Authorize]
        [Route("DeleteFriend")]
        public async Task<StatusCodeResult> DeleteFriend(string friendId)
        {
            // Валидация.
            if (string.IsNullOrWhiteSpace(friendId))
            {
                return StatusCode(400);
            }
            
            // Друг и обратный друг.    
            var currentUser = await userManager.FindByNameAsync(User.Identity.Name);
            var friend = await context.Friends.FirstOrDefaultAsync(e => e.FriendUserId == currentUser.Id && e.UserId == friendId);
            var opponentFriend = await context.Friends.FirstOrDefaultAsync(e => e.FriendUserId == friendId && e.UserId == currentUser.Id);

            if(friend == default)
            {
                return StatusCode(400);
            }
            // Если не инициатор, то запрещаем.
            if (!friend.BlockInitiator && friend.IsBlocked)
            {
                return StatusCode(205);
            }

            context.Friends.RemoveRange(new[] { friend, opponentFriend });
            await context.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region Секция СООБЩЕНИЯ отправить - создать чат
        /// <summary>
        /// Создание чата для 2х пользователей.
        /// </summary>
        /// <param name="id">ИД первого пользователя.</param>
        /// <param name="opponentId">Ид вторго пользователя.</param>
        /// <returns></returns>
        [Route("CreatePrivateChat")]
        [Authorize]
        public async Task<int> CreateUserChat(string id, string opponentId) 
        {
            // здесь добавлять проверку на заблокированность.
            // 2 потому-что для каждого из собеседников.
            var user = await userFind.GetUserByIdAsync(id);
            var opponent = await userFind.GetUserByIdAsync(opponentId);
            
            if (user == null || opponent == null)
            {
                Response.StatusCode = 204;
                return 0;
            }

            // здесь добавлять userChat если его нет.
            var createdUserChat = await context.UserChats.Include(e => e.Chat).Where(e => (e.UserId == user.Id && e.OpponentId == opponentId) || (e.UserId == opponentId && e.OpponentId == user.Id)).ToListAsync();
            if (createdUserChat.Count > 0)
            {
                if(createdUserChat.Count == 2) return createdUserChat[0].Chat.ChatId;
                UserChat newUCh;
                if (createdUserChat.Any(e => e.UserId == user.Id && e.OpponentId == opponentId)){
                    newUCh = new UserChat
                    {
                        UserId = opponentId,
                        ChatName = opponent.Name,
                        OpponentId = id,
                        ChatPhoto = opponent.Photo
                    };
                }
                else
                {
                    newUCh = new UserChat
                    {
                        UserId = id,
                        ChatName = opponent.Name,
                        OpponentId = opponentId,
                        ChatPhoto = opponent.Photo
                    };
                }
                createdUserChat[0].Chat.UserChat.Add(newUCh);
                await context.SaveChangesAsync();
                return createdUserChat[0].Chat.ChatId;
            }

            var userChatCurent = new UserChat
            {
                UserId = id,
                ChatName = opponent.Name,
                OpponentId = opponentId,
                ChatPhoto = opponent.Photo
            };
            var userChatopponent = new UserChat
            {
                UserId = opponentId,
                ChatName = user.Name,
                OpponentId = id,
                ChatPhoto = user.Photo
            };
            var chat = new Chat
            {              
                EventId = null,
                UserChat = new List<UserChat> 
                {
                    userChatCurent,
                    userChatopponent
                }
            };
            try
            {
                await context.Chats.AddAsync(chat);

                await context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);

            }
            
            return chat.ChatId;
        }

        
        /// <summary>
        /// Отправление сообщения в чат.
        /// </summary>
        /// <param name="chatId">Id чата</param>
        /// <param name="senderId">Id отправителя</param>
        /// <param name="senderName">Спевдоним отправителя</param>
        /// <param name="text">Сообщение</param>
        /// <returns></returns>
        [Route("SendTo")]
        [Authorize]
        public async Task SaveMessage(int chatId, string senderId, string senderName, string reciverId, string text)
        {
            if (senderId == "" || reciverId == "" || text == "")
                return;
            var message = new Message
            {
                ChatId = chatId,
                PersonId = senderId,
                SenderName = senderName,
                Text = text,
                PostDate = DateTime.Now,
                Read = false,
                ReciverId = reciverId
            };
            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();
        }
        #endregion

        #region Секция Получение сообщений.
        /// <summary>
        /// Получение истории сообщений.
        /// </summary>
        /// <param name="cahtId">id чата</param>
        /// <param name="lastCount">Количество нужных для загрузки записей.</param>
        /// <returns></returns>
        [Route("GetMessageHistory")]
        [Authorize]
        public async Task<List<Message>> GetMessageHistory(int chatId, int lastCount = 30)
        {
            // Реализовать количество загрузок.
            // ! Реализовать ПОЛУЧЕНИЕ ИМЕНИ ИЗ БД т.к. в изменении профиля к ссобщениям имя не меяется.
            var messages = await context.Messages.
                Where(e => e.ChatId == chatId).
                OrderByDescending(e => e.PostDate).
                Take(lastCount).OrderBy(e => e.PostDate).
                ToListAsync();
                foreach (var item in messages)
                {
                    item.Read = true;
                    context.UpdateRange(messages);                    
                }
            await context.SaveChangesAsync();
            return messages;
        }

        /// <summary>
        /// Отправка клиенту непрочитанных сообщений.
        /// </summary>
        /// <param name="chatId">ID чата.</param>
        /// <param name="opponentId">Сообщения от собеседника.</param>
        /// <returns>List<Message></returns>
        [Authorize]
        [Route("GetNewMessages")]
        public async Task<List<Message>> GetNewMessages(int chatId, string opponentId)
        {
            // ! Реализовать ПОЛУЧЕНИЕ ИМЕНИ ИЗ БД т.к. в изменении профиля к ссобщениям имя не меяется.
            var messages = await context.Messages.
                Where(e => e.ChatId == chatId && e.PersonId==opponentId && e.Read==false).
                ToListAsync();

            foreach (var e in messages)
            {
                e.Read = true;
            }
            context.Messages.UpdateRange(messages);
            await context.SaveChangesAsync();

            return messages;
        }
        /// <summary>
        /// Возвращает количество новых сообщений в чатах по chatId.
        /// </summary>
        /// <param name="chatIds"></param>
        /// <returns></returns>
        [Authorize]
        [Route("GetNewMessagesCount")]
        public async Task<List<NewMessagesVM>> GetNewMessagesCount()
        {
            // Мне кажется это криво и неэффективно. Нужно отрефакторить.
            var user = await userManager.GetUserAsync(User);
            var selection = context.UserChats.Include(e=>e.Chat).ThenInclude(e=>e.Messages).Where(e => e.UserId == user.Id).ToList();
            var rezult = new List<NewMessagesVM>();
            rezult = selection.Select(e => new NewMessagesVM
            {
                ChatId = e.ChatId,
                CountNew = e.Chat.Messages.Where(e=>e.PersonId != user.Id && e.Read == false).Count()
            }).ToList();

            return rezult;
        }
        #endregion
    }
}