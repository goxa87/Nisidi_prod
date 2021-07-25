
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
        readonly ITegSplitter tegSplitter;

        public EventSelectorService(Context _context,
            ITegSplitter _tegSplitter
            )
        {
            context = _context;
            tegSplitter = _tegSplitter;
        }

        public async Task<List<Event>> GetCostomEventsAsync(CostomSelectionArgs args)
        {
            // Фильтр
            // по дате
            var dateStart = args.DateSince;
            if (dateStart == null)
            {
                dateStart = DateTime.Now;
            }
            var dateEnd = args.DateDue;
            if (dateEnd == new DateTime(0))
            {
                dateEnd = DateTime.Now.AddDays(300);
            }
            var selection = context.Events.
                Include(e=>e.EventTegs).
                Include(e=>e.Creator).
                Include(e => e.Vizits).
                Where(e => e.Date > dateStart && e.Date < dateEnd && e.Type == EventType.Global);
            // город
            if (!string.IsNullOrWhiteSpace(args.City))
                selection = selection.Where(e => e.NormalizedCity == args.City.ToUpper());
            // заголовок
            // Возможно здесь будет тянуть из бд уже в оперативу. (узкое место нужно проверить)
            // Проверил . Судя по запросу выполняется на сервере. 
            if (!string.IsNullOrWhiteSpace(args.Title))
                selection = selection.Where(e => EF.Functions.Like(e.Title, $"%{args.Title}%"));
            var selectionLocal = selection;
            // теги
            
            if (!string.IsNullOrWhiteSpace(args.Tegs))
            {
                var tegs = tegSplitter.GetEnumerable(args.Tegs);
                selection = selection.Where(e => e.EventTegs.Any(x=> tegs.Contains(x.Teg)));
            }
            
            var selectionLocalList = await selection.OrderBy(e => e.Date).Skip(args.Skip).Take(args.Take).ToListAsync();
            /*
            foreach(var eve in selectionLocalList)
            {
                eve.Views++;
            }
            await context.SaveChangesAsync();
            */
            return selectionLocalList;
        }

        /// <summary>
        /// Выбор событий для пользователя
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Obsolete("Сейчас через js грузит")]
        public async Task<List<Event>> GetStartEventListAsync(User user)
        {
            // Это отнесено к кастомизированному поиску, т.к. по сути поже самое что и выше.
            // Не используется.
            // Идентифицированный пользователь.
            if (user != null)
            {
                var dateStart = DateTime.Now;
                var dateEnd = DateTime.Now.AddDays(100);
                var city = user.City;
                var tegs = user.Intereses;
                // город
                // период месяц
                var fromDateCity = context.Events.Include(e => e.Creator).
                    Include(e => e.EventTegs).
                    Where(e => e.City == city &&
                        e.Date > dateStart &&
                        e.Date < dateEnd &&
                        e.Type==EventType.Private).ToList();
                
                // пересечение интересов пользователя и тегов события
                var intereses = context.Intereses.Where(e => e.UserId == user.Id);
                var fromIntereses = context.EventTegs.
                    Include(e => e.Event).ThenInclude(e => e.Creator).
                    Include(e => e.Event).ThenInclude(e => e.EventTegs).
                    Join(intereses,
                        teg => teg.Teg,
                        interes => interes.Value,
                        (teg, interes) => teg.Event).ToList();
                var fromInteresesGlobal = fromIntereses.Where(e => e.Type == EventType.Private);
                return fromDateCity.Intersect(fromIntereses).OrderBy(e => e.Date).ToList();
                
            }
            else
            {
                // Не аутентифицированный пользователь.
                // Просто последние добавленные события.
                var rezult = await context.Events.Include(e=>e.Creator).Include(e=>e.EventTegs).Where(e =>
                    e.Date > DateTime.Now && e.Date < DateTime.Now.AddDays(300) &&
                    e.Type == EventType.Private).
                    OrderBy(e => e.Date).Take(40).ToListAsync();

                //var rezult = await context.Events.ToListAsync();

                return rezult;
            }
        }    
    }
}
