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

namespace EventB.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        readonly Context context;
        readonly IUserFindService userFind;
        public ApiController(
            Context _context,
            IUserFindService _userFind
            )
        {
            context = _context;
            userFind = _userFind;
        }

        #region Секция ПОДПИСКИ
        [Route("AddFriend")]
        [Authorize]
        public async Task<StatusCodeResult> AddAsFriend(string userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var currentUser = await userFind.GetCurrentUserAsync(User.Identity.Name);
                var friend = await userFind.GetUserByIdAsync(userId);

                var userFriend = new Friend
                {
                    UserId = currentUser.Id,
                    PersonFriendId = friend.Id,
                    UserName = friend.Name,
                    UserPhoto = friend.Photo
                };

                await context.Friends.AddAsync(userFriend);
                await context.SaveChangesAsync();

                return StatusCode(200);
            }
            else
            {
                return StatusCode(404);
            }           
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
            var userChatCurent = new UserChat
            {
                UserId = id,
                ChatName = opponent.Name,
                OpponentId = opponentId
            };
            var userChatopponent = new UserChat
            {
                UserId = opponentId,
                ChatName = user.Name,
                OpponentId = id
            };
            var chat = new Chat
            {
                UserChat = new List<UserChat> 
                {
                    userChatCurent,
                    userChatopponent
                }
            };

            await context.Chats.AddAsync(chat);
            await context.SaveChangesAsync();
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
            //var chat = await context.Chats.FirstOrDefaultAsync(e => e.ChatId == chatId);
            //chat.Messages.Add(message);
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

            var messages = await context.Messages.
                Where(e => e.ChatId == chatId).
                OrderByDescending(e => e.PostDate).
                Take(lastCount).OrderBy(e => e.PostDate).
                ToListAsync();
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
        #endregion
    }
}