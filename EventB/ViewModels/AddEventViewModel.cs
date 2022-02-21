using EventBLib.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels
{
    public class AddEventViewModel
    {
        [Display(Name = "Заголовок события")]
        [Required(ErrorMessage = "Поле Заголовок не заполнено")]
        [MaxLength(1000, ErrorMessage = "Длинна этого поля не должна быть больше 1000 символов")]
        public string Title { get; set; }

        [Display(Name = "Интересы")]
        [MaxLength(1000, ErrorMessage = "Длинна этого поля не должна быть больше 1000 символов")]
        public string Tegs { get; set; }

        [Display(Name = "Описание")]
        [MaxLength(4000, ErrorMessage = "Длинна этого поля не должна быть больше 4000 символов")]
        public string Body { get; set; }

        [Required(ErrorMessage = "Поле Дата и время не заполнено")]
        [Display(Name = "Дата и время")]
        public DateTime Date { get; set; }

        [Display(Name = "Город")]
        [Required(ErrorMessage = "Поле город не заполнено")]
        [MaxLength(100, ErrorMessage = "Длинна этого поля не должна быть больше 100 символов")]
        public string City { get; set; }

        [Display(Name = "Место")]
        [Required(ErrorMessage = "Поле Место проведения не заполнено")]
        [MaxLength(200, ErrorMessage = "Длинна этого поля не должна быть больше 200 символов")]
        public string Place { get; set; }

        [MaxLength(1000, ErrorMessage = "Слишком длинное, попробуйте до 1000 символов"),
            Display(Name = "Билеты (необязательно)")]
        public string TicketsDesc { get; set; }

        [MaxLength(25), Display(Name = "Телефон для связи (Необязательно)")]
        public string Phone { get; set; }

        [Display(Name = "Возрастные ограничения")]
        public int AgeRestrictions { get; set; }

        [Display(Name = "Изображение")]
        public IFormFile MainPicture { get; set; }

        [Display(Name = "Тип события")]
        public EventType Type { get; set; }
    }
}
