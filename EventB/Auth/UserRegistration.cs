using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Auth
{
    /// <summary>
    /// модель для регистрации пользователя
    /// </summary>
    public class UserRegistration
    {
        [Display(Name = "Адрес эдектронной почты")]
        [Required(ErrorMessage ="Не введен адрес"), DataType(DataType.EmailAddress, ErrorMessage ="неверный формат электронной почты")]        
        public string Email { get; set; }

        [Display(Name ="Придумате пароль не менее 6 символов")]
        [Required(ErrorMessage ="Не введен пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name ="Подтвердите Пароль отсюда")]
        [Required(ErrorMessage ="Введите пароль")]
        [DataType(DataType.Password), Compare(nameof(Password), ErrorMessage ="Пароли не совпадают")]
        public string confirmPassword { get; set; }
        /// <summary>
        /// Псевдоним
        /// </summary>
        [Display(Name ="Ваше имя(будет видно другим пользователям)")]
        [Required(ErrorMessage = "Введите пароль"), MaxLength(200, ErrorMessage ="Максимальная длина 200 символов")]
        public string Name { get; set; }

        /// <summary>
        /// город
        /// </summary>
        [Required, MaxLength(200)]
        [Display(Name ="Ваш город(поиск и уведомления будут для этого города)")]
        public string Sity { get; set; }
        /// <summary>
        /// интересы
        /// </summary>
        [MaxLength(256)]
        [Display(Name ="Ваши интересы через запятую или пробел(по ним будем искать события, подходящие для вас)")]
        public string Tegs { get; set; }
    }
}
