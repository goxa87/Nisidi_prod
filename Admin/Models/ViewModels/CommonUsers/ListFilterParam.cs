using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Models.ViewModels.CommonUsers
{
    /// <summary>
    /// Параметры фильтра для страницы списка пользователей
    /// </summary>
    public class ListFilterParam
    {
        /// <summary>
        /// Город
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ИД пользователя
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Дата регистрации ОТ
        /// </summary>
        public DateTime? RegistrationDateStart { get; set; }

        /// <summary>
        /// Дата регистрации ДО
        /// </summary>
        public DateTime? RegistrationDateEnd { get; set; }
    }
}
