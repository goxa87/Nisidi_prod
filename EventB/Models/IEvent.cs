using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB
{
    /// <summary>
    /// представляет событие (основная сущьность приложения)
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// идентификатор записи события
        /// </summary>
        int EventId { get; set; }
        /// <summary>
        /// ID автора события
        /// </summary>
        int AuthorID { get; set; }
        /// <summary>
        /// заголовок
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// ключи для поиска
        /// </summary>
        string Tegs { get; set; }
        /// <summary>
        /// описание события
        /// </summary>
        string Body { get; set; }
        /// <summary>
        /// Мето проведения
        /// </summary>
        string Sity { get; set; }
        /// <summary>
        /// место проведения
        /// </summary>
        string Place { get; set; }
        /// <summary>
        /// дата начала события 
        /// </summary>
        DateTime Date { get; set; }
        /// <summary>
        /// количество оценок нравится
        /// </summary>
        int Likes { get; set; }
        /// <summary>
        /// количество просмотров
        /// </summary>
        int Views { get; set; }
        /// <summary>
        /// количество отметок поделиться
        /// </summary>
        int Shares { get; set; }
    }
}
