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

            if (!param.StartDate.HasValue)
            {
                param.StartDate = DateTime.Now.AddDays(-45);
            }

            if (!param.EndDate.HasValue)
            {
                param.EndDate = DateTime.Now;
            }
            query = query.Where(e => e.CreationDate > param.StartDate && e.CreationDate < param.EndDate);

            if (!string.IsNullOrEmpty(param.EventTitle))
            {
                query = query.Where(e => EF.Functions.Like(e.Title, $"%{param.EventTitle.Trim()}%"));
            }

            if (!string.IsNullOrEmpty(param.UserName))
            {
                query = query.Where(e => e.Creator.NormalizedUserName == param.UserName.ToUpper() 
                    || e.Creator.NormalizedName == param.UserName.ToUpper());
            }

            // paging

            var result = new EventsListVM();
            result.PagingParam = param.PagingParam;

            var allEvents = await query.ToListAsync();
            var allEventCount = allEvents.Count;
            if (allEventCount <= param.PagingParam.PageSize)
            {
                result.PagingParam.CurrentPage = 1;
                result.PagingParam.PageCount = 1;
                result.PagingParam.TotalCount = allEventCount;
                result.Events = allEvents;
                return result;
            }

            var skip = param.PagingParam.CurrentPage * param.PagingParam.PageSize;
            if(skip > allEventCount)
            {
                skip = allEventCount - (allEventCount % param.PagingParam.PageSize);
                var take = allEventCount % param.PagingParam.PageSize;
                result.Events = allEvents.Skip(skip).Take(take).ToList();
                result.PagingParam.PageCount =(int)(allEventCount / param.PagingParam.PageSize) + 1;
                result.PagingParam.CurrentPage = (int)(allEventCount / param.PagingParam.PageSize) + 1;
                result.PagingParam.TotalCount = allEventCount;
                return result;
            }

            skip = param.PagingParam.PageSize * param.PagingParam.CurrentPage - 1;            
            result.Events = allEvents.Skip(skip).Take(param.PagingParam.PageSize).ToList();
            result.PagingParam.PageCount = (int)(allEventCount / param.PagingParam.PageSize) + 1;
            result.PagingParam.CurrentPage = (int)(allEventCount / param.PagingParam.PageSize);
            result.PagingParam.TotalCount = allEventCount;
            return result;
        }
    }
}
