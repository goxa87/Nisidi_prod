
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
                dateEnd = DateTime.Now.AddDays(90);
            }
            var selection = context.Events.
                Include(e=>e.EventTegs).
                Include(e=>e.Creator).
                Include(e => e.Vizits).
                Where(e => e.Date > dateStart && e.Date < dateEnd && e.Type == EventType.Global);
            // город
            if (!string.IsNullOrWhiteSpace(args.City))
                selection = selection.Where(e => e.NormalizedCity == args.City.Trim().ToUpper());
            // заголовок
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
            
            return selectionLocalList;
        } 
    }
}
