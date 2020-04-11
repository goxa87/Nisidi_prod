﻿using System;
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
        [MaxLength(50, ErrorMessage ="Слишком длинное название(макс 50)")]
        public string ChatName { get; set; }
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


    }
}
