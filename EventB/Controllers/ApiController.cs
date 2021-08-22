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
using EventB.Services.FriendService;
using CommonServices.Infrastructure.WebApi;
using EventB.Services.Logger;
using System.Security.Claims;

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
        readonly IFriendService friendService;

        private readonly ILogger logger;
        public ApiController(
            Context _context,
            UserManager<User> _userManager,
            ITegSplitter _tegSplitter, 
            IUserFindService _userFind,
            IFriendService _friendService,
            ILogger _logger
            )
        {
            tegSplitter = _tegSplitter;
            context = _context;
            userManager = _userManager;
            userFind = _userFind;
            friendService = _friendService;
            logger = _logger;
        }

        /// <summary>
        /// Вернет обновления для меню
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [Route("get-updates-for-menu")]
        public async Task<WebResponce<MenuUpdatesApiVM>> GetUpdatesForMenu()
        {
            if(User?.Identity?.Name == null)
            {
                return new WebResponce<MenuUpdatesApiVM>(new MenuUpdatesApiVM(), false, "неавторизован");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var HasNewFriends = await context.Friends.AnyAsync(e => e.UserId == userId && e.FriendInitiator == false && e.IsConfirmed == false && e.IsBlocked == false);
            var HasNewInvites = await context.Invites.AnyAsync(e => e.UserId == userId);
            var HasNewMessages = await context.UserChats.AnyAsync(e => e.UserId == userId && e.IsBlockedInChat == false && e.IsDeleted == false && e.Chat.Messages.Any(x => x.MessageId > e.LastReadMessageId));
            var content = new MenuUpdatesApiVM()
            {
                HasResult = true,
                HasNewFriends = HasNewFriends,
                HasNewInvites = HasNewInvites,
                HasNewMessages = HasNewMessages
            };
            var response = new WebResponce<MenuUpdatesApiVM>(content);
            return response;
        }

        #region Секция ПОДПИСКИ
        /// <summary>
        /// Блокировать пользователя.
        /// </summary>
        /// <param name="curentUserId">текущий ползователь , который блокирует.</param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        [Authorize]
        [Route("BlockUser")]
        public async Task<StatusCodeResult> BlockUser(string friendId)
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
            var friend = await context.Friends.FirstOrDefaultAsync(e => e.FriendUserId == friendId && e.UserId == currentUser.Id);
            // Запись о нас у того пользователя
            var opponentFriend = await context.Friends.FirstOrDefaultAsync(e=>e.FriendUserId == currentUser.Id && e.UserId == friendId);
            // Если не инициатор, то запрещаем.
            if (friend != null && (!friend.BlockInitiator && friend.IsBlocked ))
            {
                return StatusCode(205);
            }

            friend.IsBlocked = friend.IsBlocked ? false : true;
            friend.BlockInitiator = friend.BlockInitiator ? false : true;
            opponentFriend.IsBlocked = opponentFriend.IsBlocked ? false : true;

            // Блокируем чаты
            var chat = await context.Chats.Include(e=>e.UserChat).FirstOrDefaultAsync(e => !e.EventId.HasValue &&
                e.UserChat.Any(e=>e.UserId == friendId) &&
                e.UserChat.Any(e=>e.UserId == currentUser.Id));
            if(chat != null)
            {
                foreach (var uChat in chat.UserChat)
                {
                    uChat.IsBlockedInChat = !uChat.IsBlockedInChat;                    
                }
            }

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
            var friend = await context.Friends.FirstOrDefaultAsync(e => e.FriendUserId == friendId && e.UserId == currentUser.Id);
            var opponentFriend = await context.Friends.FirstOrDefaultAsync(e => e.FriendUserId == currentUser.Id && e.UserId == friendId);

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
        public async Task<WebResponce<int>> CreateUserChat(string id, string opponentId) 
        {
            // здесь добавлять проверку на заблокированность.
            // 2 потому-что для каждого из собеседников.
            var user = await context.Users.Include(e=>e.Friends).FirstAsync(e=>e.UserName == User.Identity.Name);
            var opponent = await userFind.GetUserByIdAsync(opponentId);
            
            if (user == null || opponent == null)
            {                
                return new WebResponce<int>(0);
            }

            // здесь добавлять userChat если его нет.
            var createdUserChat = await context.UserChats.Include(e => e.Chat).Where(e => (e.UserId == user.Id && e.OpponentId == opponentId) || (e.UserId == opponentId && e.OpponentId == user.Id)).ToListAsync();
            
            if (createdUserChat.Count > 0)
            {
                if (createdUserChat.Count == 2 && createdUserChat.All(e=>e.IsBlockedInChat == false))
                {
                    foreach(var e in createdUserChat)
                    {
                        e.IsDeleted = false;
                    }
                    await context.SaveChangesAsync();
                    return new WebResponce<int>(createdUserChat[0].Chat.ChatId);
                }
                else
                {
                    return new WebResponce<int>(0, false, "Заблокировано");
                }
            }
            var friend = user.Friends.FirstOrDefault(e => e.FriendUserId == opponentId);
            var isBlockedFriend = friend != null && friend.IsBlocked;

            var userChatCurent = new UserChat
            {
                UserId = id,
                ChatName = opponent.Name,
                OpponentId = opponentId,
                ChatPhoto = opponent.MiniImage,
                IsBlockedInChat = isBlockedFriend,
                SystemUserName = user.UserName
            };
            var userChatopponent = new UserChat
            {
                UserId = opponentId,
                ChatName = user.Name,
                OpponentId = id,
                ChatPhoto = user.MiniImage,
                IsBlockedInChat = isBlockedFriend,
                SystemUserName = opponent.UserName
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
                await logger.LogStringToFile($"CreatePrivateChat: {ex.Message}");
            }
            
            return new WebResponce<int>(chat.ChatId);
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

            var userChat = await context.UserChats.FirstOrDefaultAsync(e => e.ChatId == chatId && e.UserId == senderId);
            if (userChat.IsBlockedInChat)
            {
                HttpContext.Response.StatusCode = 403;
                return;
            }

            var message = new Message
            {
                ChatId = chatId,
                PersonId = senderId,
                SenderName = senderName,
                Text = text,
                PostDate = DateTime.Now,
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
        [Route("GetMessageListForChat")]
        [Authorize]
        public async Task<List<Message>> GetMessageHistory(int chatId, int lastCount = 10, int skip = 0)
        {
            // Реализовать количество загрузок.
            // ! Реализовать ПОЛУЧЕНИЕ ИМЕНИ ИЗ БД т.к. в изменении профиля к собщениям имя не меяется.
            var messages = await context.Messages.
                Where(e => e.ChatId == chatId).
                OrderByDescending(e => e.PostDate).
                Skip(skip).
                Take(lastCount)
                .OrderBy(e => e.PostDate).
                ToListAsync();

            return messages;
        }
        #endregion
    }
}