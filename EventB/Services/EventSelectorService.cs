
using EventBLib.DataContext;
using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services
{
    /// <summary>
    /// представляет методы для выбора событий из контекста данных по условиям
    /// </summary>
    public class EventSelectorService 
    {
        /// <summary>
        /// Метод возвращающий результат настраиваемого поиска событий
        /// </summary>
        /// <param name="args">Аргументы расширенного поиска</param>
        /// <param name="p">пользователь для которого осуществляется потиск</param>
        /// <param name="db">контекст данных</param>
        /// <returns>список событий удовлетворяющий условиям</returns>
        //public IEnumerable<Event> GetCustomEventList(CostomSelectionArgs args)
        //{            
            //IQueryable<Event> selection = db.GetEvents().Where(e => e.Sity.ToLower() == args.Sity.ToLower());


            // события друзей
            //if (args.FriendsOnly)
            //{
            //    var friendsId = args.Requester.Friends.Split(',', '.');
            //        IEnumerable<Event> selection = db.GetEvents().
            //            Where(e => e.Sity.ToLower() == (!string.IsNullOrWhiteSpace(args.Sity) ? args.Sity.ToLower() : (args.Requester.Sity.ToLower())));
            //}

            //город
            //IEnumerable<Event> selection = db.GetEvents().
            //Where(e => e.Sity.ToLower() ==
            //(!string.IsNullOrWhiteSpace(args.Sity) ?
            //    args.Sity.ToLower() :
            //    ((args.Requester != null ? args.Requester.Sity.ToLower() : "москва"))));
            ////IEnumerable<Event> selection = db.GetEvents().
            ////    Where(e => e.Sity.ToLower() == "ставрополь");

            //// дата с
            //if (args.DateSince > new DateTime(01,01,02)) selection = selection.Where(e => e.Date > args.DateSince);
            //else selection = selection.Where(e => e.Date > DateTime.Now);
            //// дата по (если не ууказано время будет 00 и нао добавить 1 день)
            //if (args.DateDue > new DateTime(01, 01, 02)) selection = selection.Where(e => e.Date < args.DateDue.AddDays(1));
            //else selection = selection.Where(e => e.Date > DateTime.Now.AddDays(7));
            ////название 
            //if (!string.IsNullOrWhiteSpace(args.Title)) selection = selection.Where(e => e.Title.Contains(args.Title));
            ////место
            //if (!string.IsNullOrWhiteSpace(args.Place)) selection = selection.Where(e => e.Place.Contains(args.Place));
            //// теги
            //if (!string.IsNullOrWhiteSpace(args.Tegs)) 
            //    selection = selection.Where(e => TegSplitter.GetEnumerable(e.Tegs).Intersect(TegSplitter.GetEnumerable(args.Tegs)).Count() > 0);

            
        //    return null;
        //}

        /// <summary>
        /// предикат для поиска событий в бд(для начальног поиска по умолчанию)
        /// должен быть либо подходящим по городу,  дате, приглашение. спец событие, совпадеие тегов 
        /// </summary>
        /// <param name="e">событие, которое проверяется на удовлетворение условиям</param>
        /// <param name="personTegs">список интересов пользователя</param>
        /// <param name="context">контекст данных</param>
        /// <param name="personId">id пользователя</param>
        /// <returns>bool (true если удовлетворяет начальным условиям заполнения)</returns>
        //bool CommonSelectionLogic(Event e, IEnumerable<string> personTegs, Context context, User p);
        //{
        //    // сокращение за счет отброса не подходящх городов
        //    //по идее эта штука должна сократить время выполнения запроса
        //    if (e.City.ToLower() == p.City.ToLower()) return false;

        //    // если приглашен взять из таблицы связи посетителей события
        //    //if (context.Vizitors.Where(y => y.EventId == e.EventId && y.PersonId == p.PersonId).Count() > 0) return true;
        //    //var x = context.Vizitors;
        //    // если специальный тип сразу в список

        //    if (e.Type == EventType.Special) return true;

        //    // если платное
        //    // если город
        //    // если дата
        //    // если теги
        //    if (e.City.ToLower() == p.City.ToLower() &&
        //        e.Type == EventType.Global &&
        //        e.Date > DateTime.Now &&
        //        TegSplitter.GetEnumerable(e.Tegs).Intersect(personTegs).Count() > 0)
        //    {
        //        return true;
        //    }

        //    else return false;
        //}

        /// <summary>
        /// Метод возвращает результат поиска по-умолчанию для текущего пользователя
        /// </summary>
        /// <param name="p">пользователь для которого будет осуществлятся поиск</param>
        /// <param name="c">контекст данных</param>
        /// <returns>список событий удовлетворяющий условиям</returns>
        //public IEnumerable<Event> GetStartEventList(User p)
        //{
            // чтобне выполнять этого в каждой итерации
            //var personTegs = TegSplitter.GetEnumerable(p.Interest);          

            //return context.Events.Where( e => CommonSelectionLogic(e, personTegs, c, p));            

            //return null;
            // пересечение 

            // вариант 2 комбинация Enumerable & Queryable
            // data
            //var personTegs = TegSplitter.GetEnumerable(p.Interest);
            //string sity = p.City.ToLower();
            //DateTime DTNow = DateTime.Now;
            //string personName = p.Name;
            //// выбор тех в которые пользователь создал сам 
            ////созданные этим пользователем
            ////IEnumerable<Event> evOwn = c.context.Events.Where(e => e.Creator == personId && e.Date > DTNow);
            //IEnumerable<Event> evOwn = c.GetEvents().Where(e => e.Creator == personName && e.Date > DTNow);

            ////события от организации для города пользователя
            ////IEnumerable<Event> evGlobals = c.context.Events.Where(e => e.Type == EventType.Special && e.Date > DTNow && e.Sity.ToLower() == sity);
            //IEnumerable<Event> evGlobals = c.GetEvents().Where(e => e.Type == EventType.Special && e.Date > DTNow && e.Sity.ToLower() == sity);
            ////остальные это для города пользователя
            ////IQueryable<Event> evSelect = c.context.Events.Where(e => e.Sity.ToLower() == sity);
            //IEnumerable<Event> evSelect = c.GetEvents().Where(e => e.Sity.ToLower() == sity);
            //// дата предстоящшие
            //evSelect = evSelect.Where(e => e.Date > DTNow);
            //// тип платные
            //evSelect = evSelect.Where(e => e.Type == EventType.Global);
            //// теги которые совпадают с тегами пользователя
            //evSelect = evSelect.Where(e => TegSplitter.GetEnumerable(e.Tegs).Intersect(personTegs).Count() > 0);

            //var rezult = evOwn.Union(evGlobals).Union(evSelect).OrderBy(e => e.Date).ToList();

            //return rezult;

            //// проверитть производительность!!! 
            //объединить в 1

        //    return null;
        //}
    }
}
