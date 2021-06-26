using CommonServices.Infrastructure.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.ViewModels.Events
{
    public class EventListParam
    {
        /// <summary>
        /// Заголовок события
        /// </summary>
        public string EventTitle { get; set; }

        /// <summary>
        /// Дата С поиска
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Дата ПО поиска
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Имя или логин пользователя
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Параметр пагинации.
        /// </summary>
        public PagingParam PagingParam { get; set; }
    }
}
