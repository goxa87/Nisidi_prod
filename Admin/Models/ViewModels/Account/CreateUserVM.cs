using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.ViewModels.Account
{
    public class CreateUserVM
    {
        [Required(ErrorMessage = "Обязательно")]
        [DataType(DataType.EmailAddress, ErrorMessage ="Формат Email")]
        [Display(Name ="Логин пользователя (Email)")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Обязательно")]
        [MinLength(6, ErrorMessage ="Минимум 6 символов")]
        [Display(Name = "Пароль для входа")]
        public string Password{ get; set; }

        [Required(ErrorMessage = "Обязательно")]
        [Display(Name = "Псевдоним пользователя в системе")]
        public string AppName { get; set; }
    }
}
