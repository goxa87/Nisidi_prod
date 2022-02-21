using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventB.ViewModels.Paging
{
    public class EventsPagingFilterModel<T, Y>
    {
        /// <summary>
        /// Элементы
        /// </summary>
        public List<T> Events { get; set; }

        /// <summary>
        /// Фильтр
        /// </summary>
        public Y Filter { get; set; }

        /// <summary>
        /// Пагинация элементов
        /// </summary>
        public PagingBaseModel Paging { get; set; }
    }
}
