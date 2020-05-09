using EventBLib.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.MyPage
{
    /// <summary>
    /// Модель для формы изменения профиля.
    /// </summary>
    public class EditProfileVM
    {
        [Display(Name = "Ваше имя(будет видно другим пользователям)")]
        [Required(ErrorMessage = "Введите псевдоним"), MaxLength(200, ErrorMessage = "Максимальная длина 200 символов")]
        public string Name { get; set; }
        /// <summary>
        /// Старое имя.
        /// </summary>
        public string OldName { get; set; }
        /// <summary>
        /// Старая фотка.
        /// </summary>
        public string OldPhoto { get; set; }
        /// <summary>
        /// Фото на аватарку.
        /// </summary>
        [Display(Name = "Ваша форография")]
        public IFormFile newPhoto { get; set; }

        /// <summary>
        /// город
        /// </summary>
        [Required, MaxLength(128, ErrorMessage = "Максимальная длина 128 символов")]
        [Display(Name = "Ваш город(поиск и уведомления будут для этого города , будьте внимательны)")]
        public string City { get; set; }
        /// <summary>
        /// интересы
        /// </summary>
        [MaxLength(1024, ErrorMessage = "Максимальная длина 1024 символа")]
        [Display(Name = "Ваши интересы через запятую или пробел(по ним будем искать события, подходящие для вас)")]
        public string Tegs { get; set; }
        /// <summary>
        /// Стары теги.
        /// </summary>
        public string OldTegs { get; set; }
        [MaxLength(1000, ErrorMessage = "Максимальная длина 1000 символа")]
        [Display(Name = "Информация о вас(до 1000 символов)")]
        public string Description { get; set; }
        /// <summary>
        /// Разрешить отправку сообщений От не друзей
        /// </summary>
        [Display(Name ="Разрешить получение сообщений от не добавленных в друзья")]
        public bool AlowAnonMessages { get; set; }
        /// <summary>
        /// Видимость для других пользователей.
        /// </summary>
        [Display(Name ="Аккаунт могут просмтривать")]
        public AccountVisible Visibility { get; set; }
        [Display(Name ="Номер телефона")]
        public string Phone { get; set; }
    }
}
