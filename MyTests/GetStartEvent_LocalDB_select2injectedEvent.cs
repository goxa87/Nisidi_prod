using EventB.Data;
using EventB.Models;
//using EventLib.SelectionServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyTests
{
    public class GetStartEvent_LocalDB_select2injectedEvent
    {
        public IDataProvider DB { get; set; }

        public GetStartEvent_LocalDB_select2injectedEvent()
        {
            DB = new DataProviderList();
            (DB as DataProviderList).InitRND(3,500_0000,0);
            (DB as DataProviderList).AddEvent(new EventB.Models.Event()
            { Sity = "москва", Creator = "2", Date = DateTime.Now.AddDays(3), Tegs = "тест", Title = "ИСКОМЫЙ по тегу городу дате", Type = EventB.Models.EventType.Global });
            (DB as DataProviderList).AddEvent(new EventB.Models.Event()
            { Sity = "москва", Creator = "2", Date = DateTime.Now.AddDays(3), Tegs = "тест", Title = "ИСКОМЫЙ по тегу", Type = EventB.Models.EventType.Global });
            (DB as DataProviderList).AddEvent(new EventB.Models.Event()
            { Sity = "ростов", Creator = "777", Date = DateTime.Now.AddDays(3), Tegs = "юююю", Title = "ИСКОМЫЙ по собственный", Type = EventB.Models.EventType.Global });
            (DB as DataProviderList).AddEvent(new EventB.Models.Event()
            { Sity = "москва", Creator = "10", Date = DateTime.Now.AddDays(3), Tegs = "юююю", Title = "ИСКОМЫЙ по специальный тип", Type = EventB.Models.EventType.Special });

        }

        public string GetRezult()
        {
            //var user = new Person() { Interest = "тест, еее", PersonId = 777, Sity = "москва" };
            //var ESelector = new EventSelectorService();
            //var selection = ESelector.GetStartEventList(user, DB);

            //StringBuilder SB = new StringBuilder();
            //SB.Append("результат выборки:\n");
            //foreach (var e in selection)
            //{
            //    SB.Append(e.Title + "\n");
            //}
            //return SB.ToString();
            return "";
        }
    }
}
