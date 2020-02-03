using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Models
{
    /// <summary>
    /// платное публичное, частное по приглашению , специальное(от администрации сайта)
    /// </summary>
    public enum EventType {Global, Private, Special};

    /// <summary>
    /// Событие - основная сущность приложения
    /// </summary>
    public class Event
    {
        /// <summary>
        /// идентификатор записи события
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// тип события из EventType (платное публичное, частное по приглашению , специальное)
        /// </summary>
        public EventType Type { get; set; }
        /// <summary>
        /// ID автора события
        /// </summary>
        [Required]
        public int Creator { get; set; }
        /// <summary>
        /// заголовок
        /// </summary>
        [Required]
        [StringLength(300), Display(Name ="Заголовок")]
        public string Title { get; set; }
        /// <summary>
        /// ключи для поиска
        /// </summary>
        public string Tegs { get; set; }
        /// <summary>
        /// адрес картинки
        /// </summary>
        public string Image { get; set; }
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
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yy hh.mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        /// <summary>
        /// Дата публикации события
        /// </summary>
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString ="{0:dd.MM.yy}", ApplyFormatInEditMode =true)]
        public DateTime CreationDate { get; set; }
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

        /// <summary>
        /// количество отметок пойду
        /// </summary>
        public int WillGo { get; set; }

        /// <summary>
        /// список пользователей с отметкой пойду
        /// </summary>
        public List<Vizitors> Vizitors { get; set; }

        /// <summary>
        /// пользователь автор публикации
        /// </summary>
        public Person PersonCreator 
        {
            get
            {
                //  логика нахожения автора публикации
                return new Person();
            }               
        }

        /// <summary>
        /// id чата привязанного к событию
        /// </summary>
        public int? ChatId{ get; set; }

        /// <summary>
        /// возвращает теги события в виде List<string>
        /// </summary>
        public List<string> TegsList 
        {
            get
            {
                // логика разбиения строки тегов на список
                return new List<string>();
            }
        }
    }
}
