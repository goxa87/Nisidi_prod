﻿
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
                Where(e => e.Date > dateStart && e.Date < dateEnd && e.Type == EventType.Private);
            // город
            if (!string.IsNullOrWhiteSpace(args.City))
                selection = selection.Where(e => e.City.ToLower() == args.City.ToLower());
            // заголовок
            if (!string.IsNullOrWhiteSpace(args.Title))
                selection = selection.Where(e => e.Title.ToLower() == args.Title.ToLower());

            var selectionLocal = selection;
            // теги
            if (!string.IsNullOrWhiteSpace(args.Tegs))
            {
                var tegs = tegSplitter.GetEnumerable(args.Tegs).ToList();
                IQueryable<Event> tegSelection = context.Events.Include(e => e.Creator).Include(e=>e.EventTegs).Where(e=>e.EventId==0);
                foreach (var teg in tegs)
                {
                    var tempSelection = context.EventTegs.Include(e=>e.Event).ThenInclude(e => e.Creator).
                        Include(e => e.Event).ThenInclude(e=>e.EventTegs).
                        Where(e => e.Teg.ToLower() == teg.ToLower()).Select(e=>e.Event);
                    tegSelection = tegSelection.Union(tempSelection);
                } 

                selectionLocal = selectionLocal.Intersect(tegSelection);
            }
            return selectionLocal.OrderBy(e=>e.Date).ToList();
        }

        /// <summary>
        /// Выбор событий для пользователя
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public async Task<List<Event>> GetStartEventListAsync(User user)
        {
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
