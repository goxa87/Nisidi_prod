using EventBLib.DataContext;
using EventBLib.Models;
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
            var eve = await context.Events.Include(e => e.Chat).FirstOrDefaultAsync(e => e.EventId == eventId);
            return await context.Messages.Where(e => e.ChatId == eve.Chat.ChatId && e.EventState == true).ToListAsync();
        }
        /// <summary>
        /// Чат события последние 30 сообщений.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task<List<Message>> GetEventChatMessages(int eventId)
        {
            var eve = await context.Events.Include(e => e.Chat).FirstOrDefaultAsync(e => e.EventId == eventId);
            return await context.Messages.Where(e => e.ChatId == eve.Chat.ChatId).Take(30).ToListAsync();
        }

    }
}
