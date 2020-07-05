using EventB.Services.MessageServices;
using EventBLib.DataContext;
using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.EventServices
{
    public class EventService : IEventService
    {
        private readonly Context context;
        private readonly IMessageService messageService;

        public EventService(Context _context,
            IMessageService _messageService)
        {
            context = _context;
            messageService = _messageService;
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
    }
}
