using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Models
{
    public class Vizit
    {
        /// <summary>
        /// ключ
        /// </summary>
        public int VizitId { get; set; }
        /// <summary>
        /// ключ пользователя
        /// </summary>        
        public string UserId { get; set; }
        /// <summary>
        /// ключ  события
        /// </summary>
        public int EventId { get; set; }
        /// <summary>
        /// пользователь
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// событие
        /// </summary>
        public Event Event { get; set; }
        /// <summary>
        /// Заголовок события.
        /// </summary>
        public string EventTitle { get; set; }
        /// <summary>
        /// Путь к картинке события.
        /// </summary>
        public string EventPhoto { get; set; }
        /// <summary>
        /// Имя Посетителя.
        /// </summary>
        public string VizitorName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string VizitirPhoto { get; set; }

        
    }
}
