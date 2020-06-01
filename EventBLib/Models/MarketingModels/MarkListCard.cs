using System;
using System.Collections.Generic;
using System.Text;

namespace EventBLib.Models.MarketingModels
{
    /// <summary>
    /// Карточка рекламы для отображения внутри списка событий.
    /// </summary>
    public class MarkListCard
    {
        /// <summary>
        /// Id карточки рекламы.
        /// </summary>
        public int MarkListCardId { get; set; }
        /// <summary>
        /// Id пользователя, котороый выложил это объявление.
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Город для отображения из поиска.
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Ссылка на изображение.
        /// </summary>
        public string ImageLink { get; set; }
        /// <summary>
        /// Ссылка для перехода по рекламе.
        /// </summary>
        public string AHref { get; set; }
        /// <summary>
        /// Оплачен ли
        /// </summary>
        public bool IsPayed { get; set; }
        /// <summary>
        /// Денежный счет для оплаты показов.
        /// </summary>
        public double PaymentAccount { get; set; }
        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreationDate { get; set; }
        /// <summary>
        /// Дата оплаты.
        /// </summary>
        public DateTime PayedDate { get; set; }
        /// <summary>
        /// Показы начиная с этого числа.
        /// </summary>
        public DateTime PublicedSince { get; set; }
        /// <summary>
        /// Показы заканчивая этим числом.
        /// </summary>
        public DateTime PublicedDue { get; set; }
        /// <summary>
        /// Количество показов.
        /// </summary>
        public int ShawnQuantity { get; set; }
        /// <summary>
        /// Тригер для поочередного показа объявления для категорий выборок.
        /// </summary>
        public int TirnTrigger { get; set; }
    }
}
