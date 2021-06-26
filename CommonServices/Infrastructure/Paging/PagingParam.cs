using System;
using System.Collections.Generic;
using System.Text;

namespace CommonServices.Infrastructure.Paging
{
    public class PagingParam
    {
        /// <summary>
        /// Размер страницы
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Общее число страниц
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// Общее число элементов
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Номер текущей страницы
        /// </summary>
        public int CurrentPage { get; set; }
    }
}
