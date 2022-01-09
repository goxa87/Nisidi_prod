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

        ///// <inheritdoc />
        //public Task<List<SupportChat>> GetUnreadSupportChats()
        //{
        //    throw new NotImplementedException();
        //}

        ///// <inheritdoc />
        //public async Task SendMessageToSupportChat(string clientId, string message, string supportPersonId, string supportName = "NISIDI.RU")
        //{
        //    var chat = await db.SupportChats.Include(e=>e.Messages).FirstOrDefaultAsync(e => e.ClientId == clientId);
        //    if(chat == null)
        //    {
        //        chat = new SupportChat()
        //        {
        //            ClientId = clientId,
        //        };
        //        db.SupportChats.Add(chat);
        //        await db.SaveChangesAsync();
        //        chat.Messages = new List<SupportMessage>();
        //    }

        //    var newMessage = new SupportMessage()
        //    {
        //        IsReadClient = false,
        //        IsReadSupport = true,
        //        MessageDate = DateTime.Now,
        //        Text = message,
        //        SupportPersonId = supportPersonId
        //    };

        //    chat.Messages.Add(newMessage);
        //    await db.SaveChangesAsync();
        //}
    }
}
