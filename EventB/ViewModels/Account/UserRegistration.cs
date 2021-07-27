using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.Account
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

        [Display(Name = "Пароль (не менее 6 символов)")]
        [Required(ErrorMessage = "Не введен пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Пароль еще раз")]
        [Required(ErrorMessage = "Повторите пароль")]
        [DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        public string СonfirmPassword { get; set; }
        /// <summary>
        /// Псевдоним
        /// </summary>
        [Display(Name = "Имя (будет видно другим пользователям)")]
        [Required(ErrorMessage = "Введите псевдоним"), MaxLength(200, ErrorMessage = "Максимальная длина 200 символов")]
        public string Name { get; set; }
        /// <summary>
        /// Фото на аватарку.
        /// </summary>
        [Display(Name = "Загрузить фото")]
        public IFormFile Photo { get; set; }

        /// <summary>
        /// город
        /// </summary>
        [Required(ErrorMessage="Введите город"), MaxLength(128,ErrorMessage ="Максимальная длина 128 символов")]
        [Display(Name = "Город (поиск событий и уведомления для этого города)")]
        public string City { get; set; }
        /// <summary>
        /// интересы
        /// </summary>
        [MaxLength(1024,ErrorMessage ="Максимальная длина 1024 символа")]
        [Display(Name = "Интересы (поможем подобрать подходящие события)")]
        public string Tegs { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Номер телефона (необязательно)")]
        public string PhoneNumber { get; set; }

        [MaxLength(1000,ErrorMessage = "Максимальная длина 1000 символов")]
        [Display(Name = "О себе (до 1000 символов)")]
        public string Description { get; set; }

        /// <summary>
        /// Согласие на обработку персональных данных.
        /// </summary>
        public bool AgreePersonalData { get; set; }
    }
}
