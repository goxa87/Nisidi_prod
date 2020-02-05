using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels
{
    public class EventSearchViewModel
    {
        /// <summary>
        ///  pзаголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Tegs { get; set; }
        /// <summary>
        /// дата начала поиска
        /// </summary>
        public DateTime DateStart { get; set; }
        /// <summary>
        /// дата окончания
        /// </summary>
        public DateTime DateEnd { get; set; }
        /// <summary>
        ///  город проведения
        /// </summary>
        public string Sity { get; set; } 

        public bool FriendsOnly { get; set; } 
        public string Place { get; set; }
    }
}
