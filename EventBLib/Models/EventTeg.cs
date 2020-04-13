using System;
using System.Collections.Generic;
using System.Text;

namespace EventBLib.Models
{
    public class EventTeg
    {
        /// <summary>
        /// Идентифоикатор.
        /// </summary>
        public int EventTegId { get; set; }
        /// <summary>
        /// Ключ для события.
        /// </summary>
        public int EventId { get; set; }
        /// <summary>
        /// Свойство связи события.
        /// </summary>
        public Event Event { get; set; }
        /// <summary>
        /// Значение тега.
        /// </summary>
        public string Teg { get; set; }
    }
}
