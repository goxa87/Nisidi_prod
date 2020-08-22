using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels
{
    /// <summary>
    /// список обновлений для меюшки
    /// </summary>
    public struct MenuUpdatesApiVM
    {
        /// <summary>
        /// Есть ли значение (для неавторизованных пользователей)
        /// </summary>
        public bool HasResult { get; set; }

        public bool HasNewFriends { get; set; }
        public bool HasNewInvites { get; set; }
        public bool HasNewMessages { get; set; }
    }
}
