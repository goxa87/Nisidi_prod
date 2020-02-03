using System;
using System.Collections.Generic;
using System.Text;
using EventB.Models;

namespace EventLib.SelectionServices
{
    /// <summary>
    /// представляет выборку экземпляров собятия
    /// </summary>
    public interface IEventSelectiorService
    {
        public IEnumerable<Event> GetStartEventList(Person p);

        public IEnumerable<Event> GetCustomEventList();
    }
}
