using Admin.Models.ViewModels.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services.EventsService
{
    /// <summary>
    /// Сервис работы с событиями
    /// </summary>
    public interface IEventsService
    {
        /// <summary>
        /// Полусить список событий по параметрам с пагинацией.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<EventsListVM> GetEventsList(EventListParam param);
    }
}
