using EventB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Data
{

    public class DbData : IDataProvider
    {


        public void AddEvent(Event e)
        {
            throw new NotImplementedException();
        }

        public void AddPerson(Person p)
        {
            throw new NotImplementedException();
        }

        public void AddVizitor(Vizitors v)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Chat> GetChat()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Event> GetEvents()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Message> GetMessage()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Person> GetPersons()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserChat> GetUserChat()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Vizitors> GetVizitors()
        {
            throw new NotImplementedException();
        }
    }
}
