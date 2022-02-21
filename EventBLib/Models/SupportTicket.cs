using EventBLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBLib.Models
{
    /// <summary>
    /// Тикеты техподдержки
    /// </summary>
    public class SupportTicket
    {
		/// <summary>
		/// Идентификатор
		/// </summary>
        public int SupportTicketId { get; set; }

        /// <summary>
        /// Тема заявки
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// Описание заявки
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Замечания
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Статус заявки
        /// </summary>
        public SupportTicketStatus Status { get; set; }

        /// <summary>
        /// Связанное событие
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Дата создания заявки
        /// </summary>
        public DateTime OpenDate { get; set; }

        /// <summary>
        /// Дата перевода в втатус в работе
        /// </summary>
        public DateTime? InWorkDate { get; set; }

        /// <summary>
        /// Дата закрытия заявки.
        /// </summary>
        public DateTime? CloseDate { get; set; }

        /// <summary>
        /// Указывает необходимо ли закрывать заявку или заявка не требует закрытия
        /// </summary>
        public bool NeedToClose { get; set; }

        /// <summary>
        /// Пользователь нисиди
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Пользователь Админки сотрудник поддержки
        /// </summary>
        public string SupportEmployeeId { get; set; }

        /// <summary>
        /// Идентификатор сотрудника поддержки в основной БД нисиди
        /// </summary>
        public string NisidiEmployeeId { get; set; }
    }
}
