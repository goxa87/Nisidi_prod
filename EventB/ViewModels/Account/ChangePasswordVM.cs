using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.Account
{
    /// <summary>
    /// ВМ для изменения пароля.
    /// </summary>
    public class ChangePasswordVM
    {
        [Display(Name="Старый пароль")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "Новый пароль")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Новый пароль еще раз")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.Password), Compare(nameof(NewPassword),ErrorMessage ="Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}
