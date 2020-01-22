using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB
{
    public class Event : IEvent
    {
        /// <summary>
        /// идентификатор записи события
        /// </summary>
        public int EventId { get; set; }
        /// <summary>
        /// ID автора события
        /// </summary>
        public int AuthorID { get; set; }
        /// <summary>
        /// заголовок
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// ключи для поиска
        /// </summary>
        public string Tegs { get; set; }
        /// <summary>
        /// описание события
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// город проведения
        /// </summary>
        public string Sity { get; set; }
        /// <summary>
        /// Мето проведения
        /// </summary>
        public string Place { get; set; }
        /// <summary>
        /// дата начала события 
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// количество оценок нравится
        /// </summary>
        public int Likes { get; set; }
        /// <summary>
        /// количество просмотров
        /// </summary>
        public int Views { get; set; }
        /// <summary>
        /// количество отметок поделиться
        /// </summary>
        public int Shares { get; set; }
    }
}
