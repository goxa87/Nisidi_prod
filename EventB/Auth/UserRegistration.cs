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

        [Required(ErrorMessage ="не введен адрес"), DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage ="Не введен пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password), Compare(nameof(Password))]
        public string confirmPassword { get; set; }
        /// <summary>
        /// Псевдоним
        /// </summary>
        [Required, MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// город
        /// </summary>
        [Required, MaxLength(200)]
        public string Sity { get; set; }
        /// <summary>
        /// интересы
        /// </summary>
        [MaxLength(256)]
        public string Tegs { get; set; }
    }
}
