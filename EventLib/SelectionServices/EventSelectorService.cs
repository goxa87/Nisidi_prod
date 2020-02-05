using EventB.Data;
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
        /// предикат для поиска событий в бд(для начальног поиска по умолчанию)
        /// должен быть либо подходящим по городу,  дате, приглашение. спец событие, совпадеие тегов 
        /// </summary>
        /// <param name="e">событие, которое проверяется на удовлетворение условиям</param>
        /// <param name="personTegs">список интересов пользователя</param>
        /// <param name="context">контекст данных</param>
        /// <param name="personId">id пользователя</param>
        /// <returns>bool (true если удовлетворяет начальным условиям заполнения)</returns>
        bool CommonSelectionLogic(Event e, IEnumerable<string> personTegs, Context context, Person p)
        {
            // сокращение за счет отброса не подходящх городов
            //по идее эта штука должна сократить время выполнения запроса
            if (e.Sity.ToLower() == p.Sity.ToLower()) return false;

            // если приглашен взять из таблицы связи посетителей события
            //if (context.Vizitors.Where(y => y.EventId == e.EventId && y.PersonId == p.PersonId).Count() > 0) return true;
            //var x = context.Vizitors;
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
        public IEnumerable<Event> GetStartEventList(Person p, IDataProvider c)
        {
            // чтобне выполнять этого в каждой итерации
            //var personTegs = TegSplitter.GetEnumerable(p.Interest);          
                       
            //return context.Events.Where( e => CommonSelectionLogic(e, personTegs, c, p));            

            //return null;
            // пересечение 

            // вариант 2 комбинация Enumerable & Queryable
            // data
            var personTegs = TegSplitter.GetEnumerable(p.Interest);
            string sity = p.Sity.ToLower();
            DateTime DTNow = DateTime.Now;
            int personId = p.PersonId;
            // выбор тех в которые пользователь создал сам 
            //созданные этим пользователем
            //IEnumerable<Event> evOwn = c.context.Events.Where(e => e.Creator == personId && e.Date > DTNow);
            IEnumerable<Event> evOwn = c.GetEvents().Where(e => e.Creator == personId && e.Date > DTNow);

            //события от организации для города пользователя
            //IEnumerable<Event> evGlobals = c.context.Events.Where(e => e.Type == EventType.Special && e.Date > DTNow && e.Sity.ToLower() == sity);
            IEnumerable<Event> evGlobals = c.GetEvents().Where(e => e.Type == EventType.Special && e.Date > DTNow && e.Sity.ToLower() == sity);
            //остальные это для города пользователя
            //IQueryable<Event> evSelect = c.context.Events.Where(e => e.Sity.ToLower() == sity);
            IEnumerable<Event> evSelect = c.GetEvents().Where(e => e.Sity.ToLower() == sity);
            // дата предстоящшие
            evSelect = evSelect.Where(e => e.Date > DTNow);
            // тип платные
            evSelect = evSelect.Where(e => e.Type == EventType.Global);
            // теги которые совпадают с тегами пользователя
            evSelect = evSelect.Where(e => TegSplitter.GetEnumerable(e.Tegs).Intersect(personTegs).Count() > 0);

            var rezult = evOwn.Union(evGlobals).Union(evSelect).OrderBy(e => e.Date).ToList();

            return rezult;

            //// проверитть производительность!!! 

        }
    }
}
