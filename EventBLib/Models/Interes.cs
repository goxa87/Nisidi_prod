using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventBLib.Models
{
    public class Interes
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int InteresId { get; set; }
        /// <summary>
        /// ИД пользователя к которому относится интерес.
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Свойство связи с пользователем к которому относится интерс.
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Строковое значение интереса.
        /// </summary>
        public string Value { get; set; }

    }
}
