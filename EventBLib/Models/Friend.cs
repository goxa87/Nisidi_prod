using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventBLib.Models
{
    public class Friend
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int FriendId { get; set; }
        /// <summary>
        /// Id пользователя , который является другом.
        /// </summary>
        public string FriendUserId { get; set; }
        /// <summary>
        /// Свойство связи (ID).
        /// </summary>
        public string  UserId { get; set; }
        /// <summary>
        /// Непосредственно друг.
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Имя Пользователя для представления в списках.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Путь до фото пользователя для отображения в списках.
        /// </summary>
        public string UserPhoto { get; set; }
        /// <summary>
        /// Флаг заблокированности (true - заблокирован) друг заблокирован.
        /// </summary>
        public bool IsBlocked { get; set; }
        /// <summary>
        /// Заявка в друзья подтверждена.
        /// </summary>
        public bool IsConfirmed { get; set; }
    }
}
