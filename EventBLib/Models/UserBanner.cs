using System;
using System.Collections.Generic;
using System.Text;

namespace EventBLib.Models
{
    /// <summary>
    /// Связь пользователя с теми баннерами, которые ему надо показать
    /// </summary>
    public class UserBanner
    {
        public int Id { get; set; }

        /// <summary>
        /// Название баннера. Будет представлять из себя имя частичного представления
        /// </summary>
        public string BannerName { get; set; }

        /// <summary>
        /// Нужно ли показывать баннер этому пользователю?
        /// </summary>
        public bool ToShow { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }
    }
}
