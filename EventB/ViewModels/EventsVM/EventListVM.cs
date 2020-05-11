using EventB.Services;
using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.EventsVM
{
    /// <summary>
    ///  ВМ для представления списка событий + параметры для будущего постраничного выбора.
    /// </summary>
    public class EventListVM
    {
        /// <summary>
        /// События
        /// </summary>
        public List<Event> events { get; set; }
        /// <summary>
        /// Параметры поиска.
        /// </summary>
        public CostomSelectionArgs args { get; set; }
    }
}
