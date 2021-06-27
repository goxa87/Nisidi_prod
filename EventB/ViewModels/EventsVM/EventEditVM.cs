using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.EventsVM
{
    public class EventEditVM
    {
        [Display(Name = "Заголовок события")]
        [Required(ErrorMessage = "Поле Заголовок не заполнено")]
        [MaxLength(1000, ErrorMessage = "Длинна этого поля не должна быть больше 1000 символов")]
        public string Title { get; set; }
        /// <summary>
        /// Ставрое имя. 
        /// </summary>
        public string OldTitle { get; set; }

        [Display(Name = "Теги тобытия")]
        [MaxLength(1000, ErrorMessage = "Длинна этого поля не должна быть больше 1000 символов")]
        public string Tegs { get; set; }
        public string OldTegs { get; set; }

        [Display(Name = "Описание")]
        [MaxLength(4000, ErrorMessage = "Длинна этого поля не должна быть больше 4000 символов")]
        public string Body { get; set; }
        public string OldBody { get; set; }

        [Display(Name = "Дата и время проведения события")]
        public DateTime Date { get; set; }
        public DateTime OldDate { get; set; }

        [Display(Name = "Город проведения")]
        [Required(ErrorMessage = "Поле город проведения не заполнено")]
        [MaxLength(100, ErrorMessage = "Длинна этого поля не должна быть больше 100 символов")]
        public string City { get; set; }
        public string OldCity { get; set; }

        [Display(Name = "Место проведения")]
        [Required(ErrorMessage = "Поле Место проведения не заполнено")]
        [MaxLength(1000, ErrorMessage = "Длинна этого поля не должна быть больше 200 символов")]
        public string Place { get; set; }
        public string OldPlace { get; set; }

        [Display(Name = "Билеты")]
        [MaxLength(1000, ErrorMessage = "Длинна этого поля не должна быть больше 1000 символов")]
        public string Tickets { get; set; }
        public string OldTickets { get; set; }

        [MaxLength(25), Display(Name = "Телефон для связи (НЕОБЯЗАТЕЛЬНО)")]
        public string Phone { get; set; }
        public string OldPhone { get; set; }
        [Display(Name ="Возрастные ограничения")]
        public int AgeRest { get; set; }

        [Display(Name = "Изображение")]
        public string MainPicture { get; set; }

        public IFormFile NewPicture { get; set; }

        public int EventId { get; set; }
    }
}
