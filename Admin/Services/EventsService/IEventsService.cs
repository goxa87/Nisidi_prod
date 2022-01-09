using Admin.Models.ViewModels.Events;
using CommonServices.Infrastructure.WebApi;
using EventBLib.Models;
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
        /// Полусить список событий по параметрам с пагинацией
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<EventsListVM> GetEventsList(EventListParam param);

        /// <summary>
        /// Вернет полное описание события
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task<Event> GetEventDetails(int eventId);

        /// <summary>
        /// Одобрить событие для показа
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task<WebResponce<string>> ConfirmEventStatus(int eventId);

        /// <summary>
        /// Заблокировать событие для показа 
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="messageToUser">Сообщение для пользователя чтобы отправить ему в чат</param>
        /// <returns></returns>
        Task<WebResponce<string>> BanEventByReason(int eventId, string messageToUser);

        // перевечти в тип частное публичное


        // удалить 
    }
}
