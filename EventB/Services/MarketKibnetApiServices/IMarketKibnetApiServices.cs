using EventB.ViewModels.MarketRoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.MarketKibnetApiServices
{
    public interface IMarketKibnetApiServices
    {
        Task<bool> ChangeEventStatus(int newType, int eventId, string userId);
        Task<bool> DeleteEvent(int eventId, string userName);
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
    }
}
