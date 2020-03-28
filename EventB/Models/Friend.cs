using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Models
{
    public class Friend
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int FriendId { get; set; }
        /// <summary>
        /// Идентификатор пользователя, который является другом.
        /// </summary>
        public string  UserId { get; set; }
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
