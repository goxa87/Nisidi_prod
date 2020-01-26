using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Models
{
    public enum ChatType { EventChat, PrivateChat, PublicChat}

    /// <summary>
    /// Представляет чат 
    /// </summary>
    public class Chat
    {
        /// <summary>
        /// Идентификатор чата
        /// </summary>
        public int ChatId { get; set; }
        /// <summary>
        /// тип беседы из enum ChatType
        /// </summary>
        [Required]
        public ChatType Type { get; set; }
        /// <summary>
        /// список сообщений входящих в чат 
        /// </summary>
        public List<Message> Messages { get; set; }
        /// <summary>
        /// список пользователей участвующих в чате
        /// </summary>
        public List<UserChat> UserChat { get; set; }
    }
}
