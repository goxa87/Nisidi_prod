using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.MessageServices
{
    public interface IMessageService
    {
        /// <summary>
        /// получение изменений события
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public Task<List<Message>> GetEventChangesMessages(int eventId);

        /// <summary>
        /// Возвращает все сообщения чата.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public Task<List<Message>> GetEventChatMessages(int eventId);
    }
}
