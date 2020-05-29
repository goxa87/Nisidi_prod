using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBLib.Models
{
    /// <summary>
    /// Платное публичное, частное по приглашению , специальное(от администрации сайта).
    /// </summary>
    public enum EventType {Global, Private, Special};

    /// <summary>
    /// Событие - основная сущность приложения.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Идентификатор записи события.
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Тип события из EventType (платное публичное, частное по приглашению , специальное).
        /// </summary>
        public EventType Type { get; set; }
        /// <summary>
        /// ID автора события.
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Свойство связзи с создателем сообщения.
        /// </summary>
        public User Creator { get; set; }
        /// <summary>
        /// Заголовок.
        /// </summary>
        [Required]
        [StringLength(300), Display(Name = "Заголовок")]
        public string Title { get; set; }
        /// <summary>
        /// Возвращает короткое название.
        /// </summary>
        public string TitleShort 
        {
            get 
            {
                if (Title.Length > 24)
                {
                    return Title.Remove(24) + "...";
                }
                else
                    return Title;
            }
        }
        /// <summary>
        /// Нормлизованный заголовок.
        /// </summary>
        [StringLength(300)]
        public string NormalizedName { get; set; }

        /// <summary>
        /// Ключи для поиска.
        /// </summary>
        public List<EventTeg> EventTegs { get; set; }
        /// <summary>
        /// Теги в строку.
        /// </summary>
        public string Tegs
        {
            // Склеивает теги в 1 строку. незабывать что нужно загружать теги чтоб это работало.
            get 
            {
                if (EventTegs == null || EventTegs.Count == 0)
                    return "нет тегов или не загружено";

                var SB = new StringBuilder();
                foreach (var teg in EventTegs)
                {
                    SB.Append(teg.Teg);
                    SB.Append(" ");
                }

                return SB.ToString();
            }
        }
        /// <summary>
        /// Адрес картинки.
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// Описание события.
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// Город проведения.
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Мето проведения.
        /// </summary>
        public string Place { get; set; }
        /// <summary>
        /// Дата начала события .
        /// </summary>
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yy hh.mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        /// <summary>
        /// Дата публикации события.
        /// </summary>
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString ="{0:dd.MM.yy}", ApplyFormatInEditMode =true)]
        public DateTime CreationDate { get; set; }
        /// <summary>
        /// Признак наличия билетов на мероприятие.
        /// </summary>
        public bool Tickets { get; set; }
        /// <summary>
        /// Описание билетов.
        /// </summary>
        public string TicketsDesc { get; set; }
        /// <summary>
        /// Количество оценок нравится.
        /// </summary>
        public int Likes { get; set; }
        /// <summary>
        /// Количество просмотров.
        /// </summary>
        public int Views { get; set; }
        /// <summary>
        /// Количество отметок поделиться.
        /// </summary>
        public int Shares { get; set; }
        /// <summary>
        /// Количество отметок пойду.
        /// </summary>
        public int WillGo { get; set; }
        /// <summary>
        /// Возрастные ограничения.
        /// </summary>
        public int AgeRestrictions { get; set; }
        /// <summary>
        /// Контактный телефон к событию.
        /// </summary>
        public string Phone { get; set; }        

        /// <summary>
        /// Список пользователей с отметкой пойду.
        /// </summary>
        public List<Vizit> Vizits { get; set; }        
               
        public Chat Chat { get; set; }
    }
}
