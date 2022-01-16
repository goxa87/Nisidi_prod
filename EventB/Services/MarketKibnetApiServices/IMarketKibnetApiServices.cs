using CommonServices.Infrastructure.WebApi;
using EventB.ViewModels.MarketRoom;
using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.MarketKibnetApiServices
{
    public interface IMarketKibnetApiServices
    {
        Task<bool> ChangeEventStatus(int newType, int eventId, string userId);

        /// <summary>
        /// Удаление события владешьцем
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> DeleteEventBYOwner(int eventId, string userId);

        /// <summary>
        /// Удаление события (администрацией)
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> DeleteEventByAdmin(int eventId, string token);

        /// <summary>
        /// Получение участников чата события
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task<List<EventUserChatMembersVM>> GetEventUserChats(int eventId);
        /// <summary>
        /// Заблокирует или разблокирует пользователя в чате.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="userId"></param>
        Task<int> SwitchUserChatBlock(int eventId, int userChatId, string curentUserId);

        /// <summary>
        /// Получить все тикеты пользователя (клиента)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WebResponce<List<SupportTicket>>> GetUserSupportTickets(string userId);

        /// <summary>
        /// Создаст новую заявку в ТП.
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="userId">Идентификатор пользователя - клиента</param>
        /// <returns></returns>
        Task<WebResponce<bool>> CreateNewSupportTicket(string message, string userId);
    }
}
