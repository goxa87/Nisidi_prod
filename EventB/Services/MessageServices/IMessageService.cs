using EventB.ViewModels.MessagesVM;
using EventBLib.Models;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Удаление чата для пользователя.
        /// </summary>
        /// <param name="UserChatId">id чата пользователя</param>
        /// <returns></returns>
        public Task<int> DeleteUserChat(int UserChatId, string userId);
        /// <summary>
        /// преобразует vmк dto
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public Message ConvertMessageVmToDTO(ChatMessageVM vm);

        /// <summary>
        /// Отметка чата кака прочитннного
        /// </summary>
        /// <param name="userChatId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task MarkAsReadMessage(int chatId, string userId);

        /// <summary>
        /// Получить количество новых непрочитанных сообщений
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<List<NewMessagesVM>> GetUnreadMessagesCount(string userId);
    }
}
