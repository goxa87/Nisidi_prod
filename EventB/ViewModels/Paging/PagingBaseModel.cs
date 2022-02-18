using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventB.ViewModels.Paging
{
    /// <summary>
    /// Базовая модель для пагинации
    /// </summary>
    public class PagingBaseModel
    {
        /// <summary>
        /// Количество элементов
        /// </summary>
        public int ItemsCount{ get; set; }
        /// <summary>
        /// Количество страниц
        /// </summary>
        public int PagesCount { get 
            {
                var pages = ItemsCount / PageSize;
                if (ItemsCount % PageSize > 0)
                {
                    pages++;
                }
                return pages;    
            }
        }

        /// <summary>
        /// Текущая страница
        /// </summary>
        public int CurrentPage{ get; set; }

        /// <summary>
        /// Размер старницы задается извне
        /// </summary>
        public int PageSize { get; set; }


        /// <summary>
        /// Наименование селектора для обработчика через js
        /// </summary>
        public string ClassSelectorName{ get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemsCount">Общее чимсло элементов</param>
        /// <param name="curentPage">Номер выбранной страницы</param>
        /// <param name="pageSize">размер страницы</param>
        /// <param name="selector">Селектор для обработчика js</param>
        public PagingBaseModel(int itemsCount, int curentPage, int pageSize, string selector)
        {
            ItemsCount = itemsCount;
            CurrentPage = curentPage;
            PageSize = pageSize;
            ClassSelectorName = selector;
        }
    }
}
