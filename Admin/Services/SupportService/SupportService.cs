using Admin.AdminDbContext;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Services.SupportService
{
    public class SupportService : ISupportService
    {

        /// <summary>
        /// Контекст БД для админки
        /// </summary>
        private readonly AdminContext adminDb;

        /// <summary>
        /// Контекст БД сайта
        /// </summary>
        private readonly Context db;

        /// <summary>
        /// Конфигурация приложения.
        /// </summary>
        private readonly IConfiguration _configuration;

        public SupportService(AdminContext _adminDb,
            Context _context,
            IConfiguration configuration)
        {
            adminDb = _adminDb;
            db = _context;
            _configuration = configuration;
        }

        /// <inheritdoc />
        public async Task<List<SupportTicket>> GetAllTickets()
        {
            return await db.SupportTickets.Where(e => e.Status != EventBLib.Enums.SupportTicketStatus.Deleted).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<SupportTicket> SaveOrUpdateTicket(SupportTicket ticket)
        {
            if(ticket.SupportTicketId == default || !(await db.SupportTickets.AnyAsync(e=>e.SupportTicketId == ticket.SupportTicketId)))
            {
                db.SupportTickets.Add(ticket);
            }
            else
            {
                db.SupportTickets.Update(ticket);
            }
            await db.SaveChangesAsync();
            
            return ticket;
        }
    }
}
