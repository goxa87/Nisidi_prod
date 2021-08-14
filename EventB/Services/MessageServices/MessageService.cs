using EventB.ViewModels.MessagesVM;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.MessageServices
{
    public class MessageService : IMessageService
    {
        private readonly Context context;
        public MessageService(Context _context)
        {
            context = _context;
        }

        /// <summary>
        /// Изменения в событии
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task<List<Message>> GetEventChangesMessages(int eventId)
        {
            var eve = await context.Events.Include(e => e.Chat).ThenInclude(e=>e.Messages).FirstOrDefaultAsync(e => e.EventId == eventId);
            var chat = await context.Chats.Include(e => e.Messages).FirstOrDefaultAsync(e => e.EventId == eventId);
            return chat.Messages.Where(e =>e.EventState == true).ToList();
        }
        /// <summary>
        /// Чат события последние 30 сообщений.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task<List<Message>> GetEventChatMessages(int eventId)
        {
            var eve = await context.Events.Include(e => e.Chat).FirstOrDefaultAsync(e => e.EventId == eventId);
            return await context.Messages.Where(e => e.ChatId == eve.Chat.ChatId).Take(15).ToListAsync();
        }

        /// <summary>
        /// Удалить чат пользователя.
        /// </summary>
        /// <param name="UserChatId"></param>
        /// <returns></returns>
        public async Task<int> DeleteUserChat(int ChatId, string userId)
        {
            var userChat = await context.UserChats
                .Include(e => e.Chat).ThenInclude(e => e.Messages)
                .Include(e=>e.User)
                .FirstOrDefaultAsync(e=>e.ChatId == ChatId && e.UserId == userId);
            if(userChat == null)
            {
                return 400;
            }

            var message = new Message
            {
                ChatId = userChat.ChatId,
                PersonId = userChat.UserId,
                SenderName = userChat.User.Name,
                Text = "Пользователь покинул чат",
                PostDate = DateTime.Now,
                Read = false
            };
            await context.Messages.AddAsync(message);
            userChat.IsDeleted = true;
            
            await context.SaveChangesAsync();
            return 200;
        }

        /// <summary>
        /// преобразует vmк dto
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public Message ConvertMessageVmToDTO(ChatMessageVM vm)
        {
            return new Message
            {
                ChatId = vm.chatId,
                EventLink = 0,
                EventLinkImage = null,
                EventState = false,
                PersonId = vm.personId,
                PostDate = vm.postDate,
                Read = false,
                ReciverId = null,
                SenderName = vm.senderName,
                Text = vm.text
            };
        }

    }
}
