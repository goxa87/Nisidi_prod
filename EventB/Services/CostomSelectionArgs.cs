﻿using EventBLib.Models;
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
        /// дата начала
        /// </summary>
        public DateTime DateSince { get; set; } = DateTime.Now;

        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime DateDue { get; set; }
        /// <summary>
        /// Пропустить.
        /// </summary>
        public int Skip { get; set; }
        /// <summary>
        /// Взять.
        /// </summary>
        public int Take { get; set; }

        public CostomSelectionArgs(DateTime DS, DateTime DE, string title = "", string city = "ставрополь", string tegs = "")
        {
            Title = title;
            City = city;
            Tegs = tegs;
            DateSince = DS;
            DateDue = DE;
        }

        public CostomSelectionArgs() { }
    }
}
