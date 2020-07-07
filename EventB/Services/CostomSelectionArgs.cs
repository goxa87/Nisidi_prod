using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services
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
        public string City { get; set; }
        /// <summary>
        /// теги события
        /// </summary>
        public string Tegs { get; set; }

        /// <summary>
        /// получены теги из профайла или из поля ввода
        /// </summary>
        public bool IsTegsFromProfile { get; set; }
        /// <summary>
        /// дата начала
        /// </summary>
        public DateTime DateSince { get; set; } = DateTime.Now;

        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime DateDue { get; set; } = DateTime.Now.AddDays(31);
        /// <summary>
        /// Пропустить.
        /// </summary>
        public int Skip { get; set; }
        /// <summary>
        /// Взять.
        /// </summary>
        public int Take { get; set; }

        public CostomSelectionArgs() { }
    }
}
