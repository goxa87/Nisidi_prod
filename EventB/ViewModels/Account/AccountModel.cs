using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.Account
{
    public class AccountModel
    {
        public string ReturnUrl { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string City { get; set; }

        public string UserName { get; set; }

        public bool AgreePersonalData { get; set; }


        /// <summary>
        /// Флаг что форма загружается второй раз
        /// </summary>
        public bool IsRepeatLoading { get; set; }

        /// <summary>
        /// Флаг что нужно показать форму регистрации
        /// </summary>
        public bool LoadRegisterPage { get; set; }
    }
}
