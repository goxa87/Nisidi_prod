using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBLib.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventBLib.Models
{
    /// <summary>
    /// Возможность просматривать аккаунт другими пользователями.
    /// </summary>
    public enum AccountVisible { Visible, FriendsOnly, Unvisible }

    public class User : IdentityUser
    {
        /// <summary>
        /// Псевдоним пользователя в системе.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Город привязки пользователя. 
        /// </summary>
        [MaxLength(50)]
        public string City { get; set; }
        /// <summary>
        /// Адрес фотографии.
        /// </summary>
        public string Photo { get; set; }
        /// <summary>
        /// Сигнализирует о возможности отправлять этому пользователю 
        /// сообщения от пользователей, которые для него не добавлены в друзья.
        /// </summary>
        public bool AnonMessages { get; set; }
        /// <summary>
        /// Видимость данных аккауната для других пользователей.
        /// </summary>
        public AccountVisible Visibility { get; set; }
        /// <summary>
        /// Несколько слов о пользователе.
        /// </summary>
        [MaxLength(1000, ErrorMessage ="Длинна этого поля не более 1000 символов")]
        public string Description { get; set; }

        public List<Event> MyEvents { get; set; }

        public List<Vizit> Vizits { get; set; }

        public List<Interes> Intereses { get; set; }

        public List<Friend> Friends { get; set; }

        public List<UserChat> UserChats { get; set; }        
    }
}
