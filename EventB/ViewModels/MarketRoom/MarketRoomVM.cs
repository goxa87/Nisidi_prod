using EventBLib.Models;
using EventBLib.Models.MarketingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.MarketRoom
{
    public class MarketRoomVM
    {
        /// <summary>
        /// Рекламный кабинет пользователя.
        /// </summary>
        public MarketKibnet MarketKibnet { get; set; }
        /// <summary>
        /// События пользователя.
        /// </summary>
        public List<Event> Events { get; set; }

        /// <summary>
        /// Баннеры.
        /// </summary>
        public List<MarketBanner> Banner { get; set; }
        
        /// <summary>
        /// Карточки листа.
        /// </summary>
        public List<MarkListCard> ListCards { get; set; }
        
        // Добавить другие тыпы рекламы.
        
    }
}
