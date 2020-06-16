using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EventBLib.Models.MarketingModels
{
    /// <summary>
    /// Рекламный баннер.
    /// </summary>
    public class MarketBanner
    {
        /// <summary>
        /// id баннера.
        /// </summary>
        public int MarketBannerId { get; set; }
        /// <summary>
        /// Ссылка для перехода.
        /// </summary>
        public string Href { get; set; }
        /// <summary>
        /// Путь к картинке баннера.
        /// </summary>
        public string BannerImage { get; set; }
        /// <summary>
        /// Заголовок для отображения поверх картинки.
        /// </summary>
        [MaxLength(250)]
        public string Title { get; set; }
    }
}
