using EventB.ViewModels;
using EventBLib.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.EventServices
{
    public interface IEventService
    {

        Task<Event> AddEvent(AddEventViewModel model, string userName);

        Task<Event> Details(int id);

        Task<int> SendMessage(string userId, string userName, int chatId, string text);
        /// <summary>
        /// Получение списка изменений в событии.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<List<Message>> GetEventMessages(int EventId);

    }
}
