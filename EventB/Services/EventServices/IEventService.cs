using EventB.ViewModels;
using EventB.ViewModels.EventsVM;
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

        Task<int> SendMessage(string userId, User user, int chatId, string text);
        /// <summary>
        /// Получение списка изменений в событии.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<List<Message>> GetEventMessages(int EventId);

        Task<int> SubmitToEvent(int eventId, string name);

        Task<int> SendLink(int eventId, int userChatId, string userName);

        Task<List<InviteOutVM>> GetFriendslist(int eventId, string userName);

        Task InviteFriendsIn(int eventId, string user,  InviteInVm[] invites);

        Task<Event> EventEdit(string userName, EventEditVM model);

        Task<int> DeleteEvent(string username, int eventId);

    }
}
