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
        /// Свойство связи с пользователем для которого друзья.
        /// </summary>
        public string  UserId { get; set; }
        /// <summary>
        /// Id пользователя , который является другом.
        /// </summary>
        public string PersonFriendId { get; set; }
        /// <summary>
        /// Имя Пользователя для представления в списках.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Путь до фото пользователя для отображения в списках.
        /// </summary>
        public string UserPhoto { get; set; }
    }
}
