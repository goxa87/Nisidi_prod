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
        [Required(ErrorMessage ="Поле Заголовок не заполнено")]
        [MaxLength(1000,ErrorMessage ="Длинна этого поля не должна быть больше 1000 символов")]
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
        [MaxLength(200, ErrorMessage = "Длинна этого поля не должна быть больше 200 символов")]
        public string Place { get; set; }

        [Display(Name = "Изображение")]
        public IFormFile MainPicture { get; set; } 
    }
}
