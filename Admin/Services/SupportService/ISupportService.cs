using Admin.Models.Enums;
using Admin.Models.ViewModels.Support;
using CommonServices.Infrastructure.WebApi;
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
        Task<List<SupportTicket>> GetTicketTickets(TicketFilterStatus type, string currentUserId);

        /// <summary>
        /// Сохранит или обновит тикет в ТП
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        Task<SupportTicket> SaveOrUpdateTicket(SupportTicket ticket);

        /// <summary>
        /// Получить детали тикета поддержки
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        Task<TicketDetailsVM> GetSupportTicketDetailsVM(int ticketId);

        /// <summary>
        /// Взять тикет в работу текущим пользователем 
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        Task<bool> AssengreTicket(string ticketId, string userId);

        /// <summary>
        /// Взять тикет в работу текущим пользователем 
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        Task<bool> SaveTicketDetails(string ticketId, string description, string note, string userId);

        /// <summary>
        /// Взять тикет в работу текущим пользователем 
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        Task<bool> CloseTicket(string ticketId, string userId);
    }
}
