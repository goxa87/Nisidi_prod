using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventBLib.Models
{
    /*
     * В общем суть в том что здесь нужно сделать ссылку на туже таблицу. 
     * UserId User это тот, кто является другом по отношению к тому кто записан в FriendUserId
     * Тоесть если мы ищем для текущего пользователя друзей, то нам нужно выбирать FriendUserId == CurrentUserId (a не userId)
     */

    public class Friend
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int FriendId { get; set; }
        /// <summary>
        /// Id пользователя друга (он будет в списке владельца)
        /// </summary>
        public string FriendUserId { get; set; }
        /// <summary>
        /// Id ползователя 
        /// </summary>
        public string  UserId { get; set; }
        /// <summary>
        /// Текущий пользователь
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
        /// Инициатор Блокировки. (true- данный пользователь, false - оппонент) 
        /// </summary>
        public bool BlockInitiator { get; set; }
        /// <summary>
        /// Заявка в друзья подтверждена.
        /// </summary>
        public bool IsConfirmed { get; set; }
        /// <summary>
        /// Инициатор дружбы( true- данный пользователь(user), false - оппонент).
        /// </summary>
        public bool FriendInitiator { get; set; }
    }
}
