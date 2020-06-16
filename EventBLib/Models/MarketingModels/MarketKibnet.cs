using System;
using System.Collections.Generic;
using System.Text;

namespace EventBLib.Models.MarketingModels
{
    /// <summary>
    /// статус рекламного кабинета.
    /// </summary>
    public enum MarketState
    {
        common,
        silver,
        gold
    }
    /// <summary>
    /// Рекламны кабинет пользователя.
    /// </summary>
    public class MarketKibnet
    {
        /// <summary>
        /// Id рекламного кабинета.
        /// </summary>
        public int MarketKibnetId { get; set; }
        /// <summary>
        /// Id пользователя к которому привязан кабинет.
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Баланс счета.
        /// </summary>
        public double PaymentAccountBalance { get; set; }
        /// <summary>
        /// Общее количество рекламных компаний. 
        /// </summary>
        public int TotalMarcetCompanyCount { get; set; }
        /// <summary>
        /// Статус рекламного кабинета.
        /// </summary>
        public MarketState MarketState { get; set; }
    }
}
