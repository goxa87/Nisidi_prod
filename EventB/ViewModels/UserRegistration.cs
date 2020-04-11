using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels
{
    /// <summary>
    /// модель для регистрации пользователя
    /// </summary>
    public class UserRegistration
    {
        [Display(Name = "Адрес электронной почты")]
        [Required(ErrorMessage = "Не введен адрес")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Неверный формат электронной почты")]
        public string Email { get; set; }

        [Display(Name = "Придумате пароль не менее 6 символов")]
        [Required(ErrorMessage = "Не введен пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Введите пароль еще раз")]
        [Required(ErrorMessage = "Повторите пароль")]
        [DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        public string СonfirmPassword { get; set; }
        /// <summary>
        /// Псевдоним
        /// </summary>
        [Display(Name = "Ваше имя(будет видно другим пользователям)")]
        [Required(ErrorMessage = "Введите псевдоним"), MaxLength(200, ErrorMessage = "Максимальная длина 200 символов")]
        public string Name { get; set; }
        /// <summary>
        /// Фото на аватарку.
        /// </summary>
        [Display(Name ="Ваша форография")]
        public IFormFile Photo { get; set; }

        /// <summary>
        /// город
        /// </summary>
        [Required, MaxLength(128,ErrorMessage ="Максимальная длина 128 символов")]
        [Display(Name ="Ваш город(поиск и уведомления будут для этого города , будьте внимательны)")]
        public string City { get; set; }
        /// <summary>
        /// интересы
        /// </summary>
        [MaxLength(1024,ErrorMessage ="Максимальная длина 1024 символа")]
        [Display(Name ="Ваши интересы через запятую или пробел(по ним будем искать события, подходящие для вас)")]
        public string Tegs { get; set; }
    }
}
