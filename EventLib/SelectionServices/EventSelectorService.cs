using EventB.DataContext;
using EventB.Models;
using EventLib.ArgsClasses;
using EventLib.StringSrevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventLib.SelectionServices
{
    

    /// <summary>
    /// представляет методы для выбора событий из контекста данных по условиям
    /// </summary>
    public class EventSelectorService : IEventSelectorService
    {
        /// <summary>
        /// Метод возвращающий результат настраиваемого поиска событий
        /// </summary>
        /// <param name="args">Аргументы расширенного поиска</param>
        /// <param name="p">пользователь для которого осуществляется потиск</param>
        /// <param name="context">сонтекст данных</param>
        /// <returns>список событий удовлетворяющий условиям</returns>
        public IEnumerable<Event> GetCustomEventList(CostomSelectionArgs args, Person p, Context context)
        {
            return null;
        }

        /// <summary>
        /// предикат для поиска событий в бд
        /// должен быть либо подходящим по городу,  дате, приглашение. спец событие, совпадеие тегов 
        /// </summary>
        /// <param name="e">событие, которое проверяется на удовлетворение условиям</param>
        /// <param name="personTegs">список интересов пользователя</param>
        /// <param name="context">контекст данных</param>
        /// <param name="personId">id пользователя</param>
        /// <returns></returns>
        bool SelectionLogic(Event e, IEnumerable<string> personTegs, Context context, Person p)
        {        
            // если приглашен взять из таблицы связи посетителей события
            if (context.Vizitors.Where(y => y.EventId == e.EventId && y.PersonId == p.PersonId).Count() > 0) return true;
            var x = context.Vizitors;
            // если специальный тип сразу в список

            if (e.Type == EventType.Special) return true;

            // если платное
            // если город
            // если дата
            // если теги
            if (e.Sity.ToLower() == p.Sity.ToLower() &&
                e.Type == EventType.Global &&
                e.Date > DateTime.Now &&
                TegSplitter.GetEnumerable(e.Tegs).Intersect(personTegs).Count() > 0)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Метод возвращает результат поиска по-умолчанию для текущего пользователя
        /// </summary>
        /// <param name="p">пользователь для которого будет осуществлятся поиск</param>
        /// <param name="context">сонтекст данных</param>
        /// <returns>список событий удовлетворяющий условиям</returns>
        public IEnumerable<Event> GetStartEventList(Person p, Context context)
        {
            var personTegs = TegSplitter.GetEnumerable(p.Interest);          
                       
            return context.Events.Where( e => SelectionLogic(e,personTegs,context, p));            

            //return null;

            // пересечение 
        }
    }
}
