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
        public string Sity { get; set; }

        /// <summary>
        /// место проведения
        /// </summary>
        public string Place { get; set; }
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
        /// часть имени создателя события
        /// </summary>
        public int Creator { get; set; }
        /// <summary>
        /// Только события друзей
        /// </summary>
        public bool FriendsOnly { get; set; }
        
        public User Requester { get; set; }

        public CostomSelectionArgs(DateTime DS, DateTime DE, string title = "", string sity = "ставрополь", string place = "", string tegs = "", int creator = -1, bool friends=false, User req=null)
        {
            Title = title;
            Sity = sity;
            Place = place;
            Tegs = tegs;
            Creator = creator;
            DateSince = DS;
            DateDue = DE;
            FriendsOnly = friends;
            Requester = req;
        }
    }
}
