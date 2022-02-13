using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.ViewModels.CommonUsers
{
    /// <summary>
    /// Модель для страницы деталей пользователя
    /// </summary>
    public class UserDetailVM
    {
        public User User { get; set; }

        public string ProfileImageUrl { get; set; }


    }
}
