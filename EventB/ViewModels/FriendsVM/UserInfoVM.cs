using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.FriendsVM
{
    // данные в представление UserInfo
    public class UserInfoVM
    {
        /// <summary>
        /// Сам пользователь.
        /// </summary>
        public User User { get; set; }

        //TODO Friend проверить что это тянет
        /// <summary>
        /// Сведения о дружбе (запись с Friends у текущего пользователя)
        /// </summary>
        public Friend Friend { get; set; }
        /// <summary>
        /// Созданные пользоватьелем события
        /// </summary>
        public List<Event> CreatedEvents { get; set; }
        /// <summary>
        /// С отметками пойду.
        /// </summary>
        public List<Vizit> WillGoEvents { get; set; }
        /// <summary>
        /// Друзья рользователя для показа в списке у кого он друг 
        /// </summary>
        public List<Friend> Friends { get; set; }
        /// <summary>
        /// Является ли другом.
        /// </summary>
        public bool IsFriend { get; set; }
    }
}
