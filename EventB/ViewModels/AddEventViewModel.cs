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

        [Display(Name = "Теги тобытия")]
        [MaxLength(1000, ErrorMessage = "Длинна этого поля не должна быть больше 1000 символов")]
        public string Tegs { get; set; }

        [Display(Name = "Описание")]
        [MaxLength(4000, ErrorMessage = "Длинна этого поля не должна быть больше 4000 символов")]
        public string Body { get; set; }

        [Required(ErrorMessage = "Поле Дата и время проведения не заполнено")]
        [Display(Name = "Дата и время проведения события")]
        public DateTime Date { get; set; }

        [Display(Name = "Город проведения")]
        [Required(ErrorMessage = "Поле город проведения не заполнено")]
        [MaxLength(100, ErrorMessage = "Длинна этого поля не должна быть больше 100 символов")]
        public string City { get; set; }

        [Display(Name = "Место проведения")]
        [Required(ErrorMessage = "Поле Место проведения не заполнено")]
        [MaxLength(1000, ErrorMessage = "Длинна этого поля не должна быть больше 200 символов")]
        public string Place { get; set; }

        [MaxLength(1000, ErrorMessage = "Слишком длинное, попробуйте до 1000 символов"), Display(Name = "Если нужны билеты, опишите что и как")]
        public string TicketsDesc { get; set; }

        [MaxLength(25), Display(Name ="Телефон для связи (НЕОБЯЗАТЕЛЬНО)")]
        public string Phone { get; set; }

        [Display(Name = "Возрастные ограничения мероприятия (Обязательно для публичных мероприятий)")]
        public int AgeRestrictions { get; set; }

        [Display(Name = "Изображение")]
        public IFormFile MainPicture { get; set; } 
    }
}
