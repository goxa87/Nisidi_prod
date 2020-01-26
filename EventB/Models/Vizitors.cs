using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Models
{
    /// <summary>
    /// представляет связь между сущностями событие и пользователь - многие ко многим
    /// </summary>
    public class Vizitors
    {
        /// <summary>
        /// ключ
        /// </summary>
        public int VizitorsId { get; set; }
        /// <summary>
        /// ключ пользователя
        /// </summary>        
        public int PersonId { get; set; }
        /// <summary>
        /// ключ  события
        /// </summary>
        public int EventId { get; set; }
        /// <summary>
        /// пользователь
        /// </summary>
        public Person Person { get; set; }
        /// <summary>
        /// событие
        /// </summary>
        public Event Event { get; set; }
    }
}
