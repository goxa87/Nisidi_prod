using System;
using System.Collections.Generic;
using System.Text;

namespace EventBLib.Models
{
    /// <summary>
    /// Сообщение в техподдержку
    /// </summary>
    [Obsolete("Пока решил выпилить это и перейти на систему Заявок")]
    public class SupportMessage
    {
        public int SupportMessageId { get; set; }
        public string Text { get; set; }        
        public DateTime MessageDate { get; set; }
        public User Client { get; set; }

        /// <summary>
        /// Id Пользователя
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Пользователь
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// Последнее сообщение прочитано пользователем
        /// </summary>
        public bool IsReadClient { get; set; }

        /// <summary>
        /// Идентификатор работника техподдержки
        /// </summary>
        public string SupportPersonId { get; set; }
        /// <summary>
        /// Последнее сообщение пользователя прочитано техподдержкой
        /// </summary>
        public bool IsReadSupport { get; set; }

        /// <summary>
        /// Навигация к чату
        /// </summary>
        public int SupportChatId { get; set; }
    }
}
