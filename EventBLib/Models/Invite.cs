using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EventBLib.Models
{
    /// <summary>
    /// Представляет приглашение пользователя на событие от другого пользователя.
    /// </summary>
    public class Invite
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int InviteId { get; set; }
        /// <summary>
        /// ИД события к которому привязано приглашение.
        /// </summary>
        [Required]
        public int EventId { get; set; }
        /// <summary>
        /// Свойство связи с событием.
        /// </summary>
        public Event Event { get; set; }
        /// <summary>
        /// ИД пользователя , который приглашен на событие.
        /// </summary>
        [Required]
        public string UserId { get; set; }
        /// <summary>
        /// Свойство связи с пользователем.
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// ИД пользователя который создал приглашение.
        /// </summary>
        [Required]
        public string InviterId { get; set; }
        /// <summary>
        /// Имя пригласившего.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string InviterName { get; set; }
        /// <summary>
        /// Фото пригласившего.
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string InviterPhoto { get; set; }
        /// <summary>
        /// Текст приглашения.
        /// </summary>
        [MaxLength(1000)]
        public string InviteDescription { get; set; }
    }
}
