using System;
using System.Collections.Generic;
using System.Text;

namespace EventBLib.Models
{
    /// <summary>
    /// Чат техподдержки который хранит сообщения пользователя
    /// </summary>
    [Obsolete("Пока решил выпилить это и перейти на систему Заявок")]
    public class SupportChat
    {
        public int SupportChatId { get; set; }

        /// <summary>
        /// Чат может быть не взят в работу сотрудником поэтому это поле может быть пустым
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Работник техподдержки
        /// </summary>
        public User SupportPerson { get; set; }

        /// <summary>
        /// Id пользователя который пишет в поддержку.
        /// </summary>
        public string ClientId { get; set; }

        public List<SupportMessage> Messages { get; set; }


    }
}
