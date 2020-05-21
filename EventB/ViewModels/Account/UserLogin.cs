using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.Account
{
    public class UserLogin
    {
        [Required(ErrorMessage ="Не заполнено поле \"Логин\"")]
        [Display(Name ="Адрес электронной почты:")]
        public string LoginProp { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Не заполнено поле \"Пароль\"")]
        [Display(Name = "Пароль:")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
