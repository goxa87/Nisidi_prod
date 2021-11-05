using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.EventsVM
{
    /// <summary>
    /// Данные для запроса по рекомендациям
    /// </summary>
    public class ByRecomendationsProps
    {
        /// <summary>
        /// Город (не актуально для авторихованного)
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Список интересов для запроса
        /// </summary>
        public List<string> Intereses { get; set; }
    }
}
