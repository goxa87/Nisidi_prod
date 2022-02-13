using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.ViewModels.CommonUsers
{
    /// <summary>
    /// Модель для списка пользователей
    /// </summary>
    public class ListVM
    {
        /// <summary>
        /// Параметры фильтра
        /// </summary>
        public ListFilterParam FilterParam { get; set; }

        /// <summary>
        /// Список пользователей в выдаче
        /// </summary>
        public List<User> Users { get; set; }
    }
}
