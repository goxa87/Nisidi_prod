using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.EventsVM
{
    /// <summary>
    /// Модель для отправки во фронт.
    /// </summary>
    public class InviteOutVM
    {
        //ИД пользователя.
        public string UserId { get; set; }
        /// <summary>
        /// фото пользователя.
        /// </summary>        
        public string Photo { get; set; }
        /// <summary>
        /// имя пользователя.
        /// </summary>
        public string Name { get; set; }
    }
}
