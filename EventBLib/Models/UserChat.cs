using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventBLib.Models
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
        /// Название чата которое будет отражаться в списке чатов.
        /// </summary>
        public string ChatName { get; set; }
        /// <summary>
        /// Изображение чата.
        /// </summary>
        public string ChatPhoto { get; set; }
        /// <summary>
        /// ID собеседника из приватного чата.
        /// </summary>
        public string OpponentId { get; set; }
        /// <summary>
        /// id пользователя.
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// id чата.
        /// </summary>
        public int ChatId { get; set; }
        /// <summary>
        /// Пользователь.
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Чат.
        /// </summary>
        public Chat Chat { get; set; }
        /// <summary>
        /// Сигнализирует о том что пользователь заблокирован в чате
        /// </summary>
        public bool IsBlockedInChat { get; set; }
        /// <summary>
        /// сигнализирует о том, что пользователь покинул чат
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Системное имя пользователя(логин) ТОго пользователя, которому принадлежит юзерчат
        /// </summary>
        public string SystemUserName { get; set; }

        /// <summary>
        /// ID последнего прочитанного сообщения. 
        /// </summary>
        public int LastReadMessageId { get; set; }

        /// <summary>
        /// Id события к кторому принадлежит чат
        /// </summary>
        public string RealtedObjectLink { get; set; }
    }
}
