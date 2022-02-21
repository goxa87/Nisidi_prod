using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventB.Services
{
    /// <summary>
    /// Сервис всяких настроек
    /// </summary>
    public class SettingsService
    {
        public SettingsService()
        {

        }


        /// <summary>
        /// Стандартный размер страницы пагинации
        /// </summary>
        public int DefaultPagingPageSize {
            get
            {
                return 24;
            }
        }
    }
}
