
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services
{
    /// <summary>
    /// представляет методы для выбора событий из контекста данных по условиям
    /// </summary>
    public class EventSelectorService : IEventSelectorService
    {
        readonly Context context;
        readonly IUserFindService findService;

        public EventSelectorService(Context _context,
            IUserFindService _findService)
        {
            context = _context;
            IUserFindService findService = _findService;
        }

        public async Task<IEnumerable<Event>> GetCostomEventsAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Выбор событий для пользователя
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Event>> GetStartEventListAsync(User user)
        {
            var dateStart = DateTime.Now;
            var dateEnd = DateTime.Now.AddDays(30);
            var city = user.City;
            var tegs = user.Intereses;
            // город
            // период месяц
            var fromDateCity = context.Events.Include(e=>e.Creator).
                Include(e=>e.EventTegs).
                Where(e => e.City == city && e.Date > dateStart && e.Date < dateEnd);
            // подписался пойду
            var fromVizits = context.Vizits.Include(e => e.Event).ThenInclude(e=>e.EventTegs).
                Include(e => e.Event).ThenInclude(e=>e.Creator).
                Where(e => e.UserId == user.Id).
                Select(e => e.Event);

            var imRez = fromDateCity.Intersect(fromVizits);
            // пересечение интересов пользователя и тегов события
            var intereses = context.Intereses.Where(e => e.UserId == user.Id);
            var fromIntereses = context.EventTegs.Include(e => e.Event).ThenInclude(e=>e.Creator).
                Include(e=>e.Event).ThenInclude(e=>e.EventTegs).
                Join(intereses, teg => teg.Teg, interes => interes.Value, (teg, interes) => teg.Event);

            return await imRez.Intersect(fromIntereses).OrderBy(e=>e.Date).ToListAsync();
        }    
    }
}
