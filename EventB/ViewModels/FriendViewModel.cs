using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels
{
    /// <summary>
    /// Класс представления во вью для различного отображения блоков
    /// </summary>
    public class FriendViewModel
    {
        // Собственно запись о друге
        public Friend Friend { get; set; }
        // Является ли он текущим другом, чтобы изменять отображение функцинальных элементов
        public bool IsFriend { get; set; }
    }
}
