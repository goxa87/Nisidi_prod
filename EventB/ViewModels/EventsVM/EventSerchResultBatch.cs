using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.EventsVM
{
    /// <summary>
    /// Модель для выдачи списка событий с ДОП содержимым
    /// </summary>
    public class EventSerchResultBatch
    {
        /// <summary>
        /// Список событий для выдачи
        /// </summary>
        public List<Event> Events { get; set; }

        /// <summary>
        /// Список шаблонов и их позиций в на странице
        /// </summary>
        public Dictionary<int, string> TemplatesWithPositions { get; set; }
    }
}
