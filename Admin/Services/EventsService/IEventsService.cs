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
        /// <param name="employeeId">Id сотрудника техподдержки</param>
        /// <returns></returns>
        Task<WebResponce<string>> BanEventByReason(int eventId, string messageToUser, string employeeId);

        /// <summary>
        /// Удалить событие безвозвратно.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WebResponce<bool>> DeleteEvent(int eventId, string userId);

        /// <summary>
        /// Спиок всех событий пользователя
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<List<Event>> GetEventListByUserId(string userId, bool noTracking);
    }
}
