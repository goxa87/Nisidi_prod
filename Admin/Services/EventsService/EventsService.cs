using Admin.AdminDbContext;
using Admin.Models.ViewModels.Events;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services.EventsService
{

    public class EventsService : IEventsService
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

        public EventsService(AdminContext _adminDb,
            Context _context,
            IConfiguration configuration)
        {
            adminDb = _adminDb;
            db = _context;
            _configuration = configuration;
        }

        ///<inheritdoc />
        public async Task<Event> GetEventDetails(int eventId)
        {
            var eve = await db.Events.Include(e=>e.EventTegs).Include(e=>e.Creator).Include(e=>e.Vizits).FirstOrDefaultAsync(e=>e.EventId == eventId);
            eve.Image = _configuration.GetValue<string>("RootHostnisidi") + eve.Image;
            eve.Image = _configuration.GetValue<string>("RootHostnisidi") + "/images/EventImages/ab69d908-5420-4fe0-a322-b1126b890d23.jpeg";
            return eve;
        }

        ///<inheritdoc />
        public async Task<EventsListVM> GetEventsList(EventListParam param)
        {
            var query = db.Events.Include(e => e.EventTegs).Include(e=>e.Creator).AsQueryable();

            if (param.EventCreateStartDate.HasValue)
            {
                query = query.Where(e => e.CreationDate > param.EventCreateStartDate);
            }

            if (param.EventCreateEndDate.HasValue)
            {
                query = query.Where(e => e.CreationDate < param.EventCreateEndDate);
            }
            var i = await query.ToListAsync();
            if (param.StartDate.HasValue)
            {
                query = query.Where(e => e.Date >= param.StartDate);
            }
             i = await query.ToListAsync();

            if (param.EndDate.HasValue)
            {
                query = query.Where(e => e.Date <= param.EndDate);
            }
            i = await query.ToListAsync();
            if (!string.IsNullOrEmpty(param.EventTitle))
            {
                query = query.Where(e => EF.Functions.Like(e.Title, $"%{param.EventTitle.Trim()}%"));
            }

            if (!string.IsNullOrEmpty(param.UserName))
            {
                query = query.Where(e => e.Creator.NormalizedUserName == param.UserName.ToUpper() 
                    || e.Creator.NormalizedName == param.UserName.ToUpper());
            }

            if (param.IsGlobal)
            {
                query.Where(e => e.Type == EventBLib.Models.EventType.Global);
            }

            return new EventsListVM() { Events = await query.ToListAsync(), SearchParam = param };
        }
    }
}
