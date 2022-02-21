using Admin.AdminDbContext.Models;
using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.ViewModels.Support
{
    public class TicketDetailsVM
    {
        /// <summary>
        /// Описание тикета
        /// </summary>
        public SupportTicket Ticket { get; set; }

        /// <summary>
        /// Пользователь админки
        /// </summary>
        public AdminUser AdminUser { get; set; }

        /// <summary>
        /// Пользователь нисиди
        /// </summary>
        public User NisidiUser { get; set; }

        /// <summary>
        /// Url основного сайта
        /// </summary>
        public string NisidiUserUrl { get; set; }
    }
}
