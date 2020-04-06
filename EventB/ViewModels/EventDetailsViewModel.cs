using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels
{
    public class EventDetailsViewModel
    {
        /// <summary>
        /// событие
        /// </summary>
        public Event Event { get; set; }
        /// <summary>
        /// словарь посетителей (имя - id)
        /// </summary>
        public Dictionary<int, string> Vizitors { get; set; }
        /// <summary>
        /// сообщения этого события
        /// </summary>
        public List<Message> Messages { get; set; }
        /// <summary>
        /// имя автора публикации
        /// </summary>
        public string CreatorName { get; set; }

    }
}
