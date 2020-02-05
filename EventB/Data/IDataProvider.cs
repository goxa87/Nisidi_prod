using EventB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Data
{
    public interface IDataProvider
    {
        DataContext.Context context { get; set; }

        public IEnumerable<Event> GetEvents();
        public IEnumerable<Person> GetPersons();
        public IEnumerable<Chat> GetChat();
        public IEnumerable<Message> GetMessage();
        public IEnumerable<UserChat> GetUserChat();
        public IEnumerable<Vizitors> GetVizitors();

        void AddEvent(Event e);
        void AddPerson(Person p);
        void AddVizitor(Vizitors v);

    }
}
