using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Services.SupportService
{
    public interface ISupportService
    {
        /// <summary>
        /// Получить все заявки
        /// </summary>
        /// <returns></returns>
        Task<List<SupportTicket>> GetAllTickets();

        /// <summary>
        /// Сохранит или обновит тикет в ТП
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        Task<SupportTicket> SaveOrUpdateTicket(SupportTicket ticket);
    }
}
