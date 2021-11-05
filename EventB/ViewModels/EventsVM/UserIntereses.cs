using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.EventsVM
{
    /// <summary>
    /// Интересы пользоавателя
    /// </summary>
    public class UserIntereses
    {
        /// <summary>
        /// Инициализауия интересы пользователя
        /// </summary>
        public UserIntereses()
        {
            Intereses = new List<string>();
        }

        /// <summary>
        /// ID Пользователя
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Список интересов
        /// </summary>
        public List<string> Intereses { get; set; }
    }
}
