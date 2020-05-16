using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels
{
    public class MessagesViewModel
    {
        /// <summary>
        /// UserChats для пользователя в которых информация для списка собеседников.
        /// </summary>
        public List<UserChat> userChats { get; set; } = new List<UserChat>();
        /// <summary>
        /// Список сообщений если чат под пользователя.
        /// </summary>
        public List<Message> Messages { get; set; } = new List<Message>();
        /// <summary>
        /// Id текущего пользователя.
        /// </summary>
        public string CurrentUserId { get; set; }
        /// <summary>
        /// Имя текушего пользователя.
        /// </summary>
        public string CurrentUserName { get; set; }
        /// <summary>
        /// Собеседник, если он выбран.
        /// </summary>
        public User Opponent { get; set; }
        /// <summary>
        /// Id чата если создается для конкретного чата.
        /// </summary>
        public int? CurrentChatId { get; set; }
    }
}
