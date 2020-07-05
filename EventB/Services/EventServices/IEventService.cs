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
        /// <summary>
        /// Получение списка изменений в событии.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<List<Message>> GetEventMessages(int EventId);

    }
}
