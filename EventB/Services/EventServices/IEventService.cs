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
        /// <summary>
        /// Добавить событие
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<Event> AddEvent(AddEventViewModel model, string userName);

        /// <summary>
        /// детали события
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Event> Details(int id);

        /// <summary>
        /// Отправить сообщение в чат события
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <param name="chatId"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<int> SendMessage(string userId, User user, int chatId, string text);

        /// <summary>
        /// Получение списка изменений в событии.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<List<Message>> GetEventMessages(int EventId);

        /// <summary>
        /// Подтвердить участие
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<int> SubmitToEvent(int eventId, string name);

        /// <summary>
        /// Отпарить ссылку на событие
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="userChatId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<int> SendLink(int eventId, int userChatId, string userName);

        /// <summary>
        /// Получить список друзей
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<List<InviteOutVM>> GetFriendslist(int eventId, string userName);

        /// <summary>
        /// Пригласить друга
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="user"></param>
        /// <param name="invites"></param>
        /// <returns></returns>
        Task InviteFriendsIn(int eventId, string user,  InviteInVm[] invites);

        /// <summary>
        /// Редактировать событие
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Event> EventEdit(string userName, EventEditVM model);
    }
}
