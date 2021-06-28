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
        /// Дата создания события с
        /// </summary>
        public DateTime? EventCreateStartDate { get; set; }
        
        /// <summary>
        /// дата создания события по
        /// </summary>
        public DateTime? EventCreateEndDate { get; set; }

        /// <summary>
        /// Дата С проведения поиска
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Дата ПО проведения поиска
        /// </summary>
        public DateTime? EndDate { get; set; }




        /// <summary>
        /// Имя или логин пользователя
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Публичные события.
        /// </summary>
        public bool IsGlobal { get; set; }



    }
}
