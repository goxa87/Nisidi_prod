﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventBLib.Models
{
    /// <summary>
    /// Представляет сообщение в чате.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Идентификатор сообщения.
        /// </summary>
        public int MessageId { get; set; }
        /// <summary>
        /// Идентификатор чата к которому относится сообщение.
        /// </summary>
        [Required]
        public int ChatId { get; set; }
        /// <summary>
        /// Идентификатор автора сообщения.
        /// </summary>
        [Required]
        public string PersonId { get; set; }
        /// <summary>
        /// Имя пользователя для быстрого доступа.
        /// </summary>
        public string SenderName { get; set; }
        /// <summary>
        /// Имя получателя сообщения (для приватных чатов).
        /// </summary>
        public string ReciverId { get; set; }
        /// <summary>
        /// Текст сообщения.
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Дата публикации сообщения.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString ="{0:dd.MM.yy hh.mm}",ApplyFormatInEditMode =true)]
        public DateTime PostDate { get; set; }
        /// <summary>
        /// Cимвол прочитанности.
        /// </summary>
        public bool Read { get; set; }
    }
}