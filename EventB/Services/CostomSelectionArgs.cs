using System;
using System.Collections.Generic;
using System.Text;

namespace EventLib.ArgsClasses
{
    public class CostomSelectionArgs
    {
        /// <summary>
        /// поиск по содержимому тегов или заголовков
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Город
        /// </summary>
        public string Sity { get; set; }

        /// <summary>
        /// дата начала
        /// </summary>
        public DateTime DateSince { get; set; }
       
        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime DatDue { get; set; }

        /// <summary>
        /// часть имени создателя события
        /// </summary>
        public string Creator { get; set; }
    }
}
