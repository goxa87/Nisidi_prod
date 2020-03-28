using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Models
{
    /// <summary>
    /// представляет чаты пользователя многие ко многим
    /// </summary>
    public class UserChat
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int UserChatId { get; set; }
        /// <summary>
        /// id пользователя
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// id чата
        /// </summary>
        public int ChatId { get; set; }
        /// <summary>
        /// Пользователь
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Чат
        /// </summary>
        public Chat Chat { get; set; }


    }
}
