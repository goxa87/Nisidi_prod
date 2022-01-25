using Admin.AdminDbContext;
using Admin.Models.Enums;
using Admin.Models.ViewModels.Support;
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
        public async Task<List<SupportTicket>> GetTicketTickets(TicketFilterStatus type, string currentUserId)
        {
            var result = db.SupportTickets.Where(e => e.Status != EventBLib.Enums.SupportTicketStatus.Deleted);
            switch (type)
            {
                case TicketFilterStatus.all:
                    break;
                case TicketFilterStatus.notNeedToClose:
                    result = result.Where(e => e.NeedToClose == false);
                    break;
                case TicketFilterStatus.theNew:
                    result = result.Where(e => e.Status == EventBLib.Enums.SupportTicketStatus.New && e.NeedToClose);
                    break;
                case TicketFilterStatus.currentUsers:
                    result = result.Where(e => e.SupportEmployeeId == currentUserId);
                    break;
                case TicketFilterStatus.currentUsersInWork:
                    result = result.Where(e => e.SupportEmployeeId == currentUserId && e.InWorkDate.HasValue && !e.CloseDate.HasValue);
                    break;
            }
            return await result.OrderByDescending(e=>e.SupportTicketId).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<TicketDetailsVM> GetSupportTicketDetailsVM(int ticketId)
        {
            var nisidiUrl = _configuration.GetValue<string>("RootHostNisidi");
            var ticket = await db.SupportTickets.FirstAsync(e=>e.SupportTicketId == ticketId);
            var nisidiUser = await db.Users.FindAsync(ticket.UserId);
            var adminUser = await adminDb.Users.FirstOrDefaultAsync(e=>e.Id == ticket.SupportEmployeeId);

            return new TicketDetailsVM()
            {
                Ticket = ticket,
                NisidiUserUrl = nisidiUrl + "Friends/UserInfo?userId=" + ticket.UserId,
                NisidiUser = nisidiUser,
                AdminUser = adminUser
            };
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

        public async Task<bool> AssengreTicket(string ticketId, string userId)
        {
            var id = Int32.Parse(ticketId);
            var ticket = await db.SupportTickets.FirstAsync(e => e.SupportTicketId == id);
            ticket.SupportEmployeeId = userId;
            ticket.InWorkDate = DateTime.Now;
            ticket.Status = EventBLib.Enums.SupportTicketStatus.InWork;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SaveTicketDetails(string ticketId, string description, string note, string userId)
        {
            var id = Int32.Parse(ticketId);
            var ticket = await db.SupportTickets.FirstAsync(e => e.SupportTicketId == id);
            ticket.Description = description;
            ticket.Note = note;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CloseTicket(string ticketId, string userId)
        {
            var id = Int32.Parse(ticketId);
            var ticket = await db.SupportTickets.FirstAsync(e => e.SupportTicketId == id);
            ticket.CloseDate = DateTime.Now;
            ticket.Status = EventBLib.Enums.SupportTicketStatus.Closed;
            await db.SaveChangesAsync();
            return true;
        }
    }
}
