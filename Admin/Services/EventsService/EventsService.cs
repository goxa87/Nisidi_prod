using Admin.AdminDbContext;
using Admin.Models.ViewModels.Events;
using EventBLib.DataContext;
using Microsoft.EntityFrameworkCore;
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
        private readonly Context context;

        public EventsService(AdminContext _adminDb,
            Context _context)
        {
            adminDb = _adminDb;
            context = _context;
        }

        ///<inheritdoc />
        public async Task<EventsListVM> GetEventsList(EventListParam param)
        {
            // filter
            var query = context.Events.Include(e => e.EventTegs).Include(e=>e.Creator).AsQueryable();

            if (!param.EventCreateStartDate.HasValue)
            {
                param.EventCreateStartDate = DateTime.Now.AddDays(-45);
            }

            if (!param.EventCreateEndDate.HasValue)
            {
                param.EventCreateEndDate = DateTime.Now;
            }
            query = query.Where(e => e.CreationDate > param.EventCreateStartDate && e.CreationDate < param.EventCreateEndDate);

            if (param.StartDate.HasValue)
            {
                query.Where(e => e.Date >= param.StartDate);
            }

            if (param.EndDate.HasValue)
            {
                query.Where(e => e.Date <= param.EndDate);
            }

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
