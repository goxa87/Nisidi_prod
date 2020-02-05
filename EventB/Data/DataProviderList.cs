using EventB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace EventB.Data
{
    enum tegs {концерт, туса, выставка, собрание, встреча, митинг, шествие, опера, балет, }


    public class DataProviderList : IDataProvider
    {
        public List<Event> Events { get; set; }
        public List<Person> Persons { get; set; }
        public List<Vizitors> Vizitors { get; set; }

        private char[] separators = { ' ', '_', '-', '+', '/', ',', '.', '|', '\\', ':', ';' };
        private string[] tegs = { "концерт", "туса", "выставка", "собрание", "встреча", "митинг", "шествие", "опера", "балет" };
        private string[] names = { "Вася", "Петя", "Жорик", "Себастьсян", "Таня", "Катя", "Света", "Наташа", "Гоша" };
        public DataProviderList()
        {
            Events = new List<Event>();
            Persons = new List<Person>();
            Vizitors = new List<Vizitors>();
        }

        #region fillData
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nP">колво пользователей</param>
        /// <param name="nE">колво событий</param>
        /// <param name="nV">колво связей</param>
        public void InitRND(int nP, int nE, int nV)
        {
            int tegMax = tegs.Length;
            int separatorMax = separators.Length;

            Random rnd = new Random();
            // люди
            for (int i = 1; i <=nP; i++)
            {
                Persons.Add(new Person { 
                    PersonId=i,
                    Name = $"{names[rnd.Next(names.Length)]}{i.ToString()}",
                    Email = "",
                    Role="",
                    Sity=i%2==0?"Ставролполь":"Краснодар",
                    Interest=$"{tegs[rnd.Next(tegMax)]} {separators[rnd.Next(separators.Length)]}{tegs[rnd.Next(tegMax)]}{separators[rnd.Next(separators.Length)]} {tegs[rnd.Next(tegMax)]}",
                    Friends="1,2,3,4"
                });
            }
            //события
            for (int i = 0; i <= nE; i++)
            {
                Events.Add(new Event
                {
                    EventId = i,
                    Type = i % 3 == 0 ? EventType.Global : EventType.Private,
                    Creator = i % 4,
                    Title = $"Заголовок {i}",
                    Body = $"тело {i}",
                    Sity = i % 5 < 2 ? "Ставрополь" : "Краснодар",
                    Tegs = $"{tegs[rnd.Next(tegMax)]} {separators[rnd.Next(separators.Length)]}{tegs[rnd.Next(tegMax)]}",
                    Date = DateTime.Now.AddDays(5)
                });
            }
        }

        #endregion

        #region output

        public string GetPersonById(int id) => $" {Persons[id - 1].PersonId} {Persons[id - 1].Name} " +
            $"{Persons[id - 1].Sity}\n  {Persons[id - 1].Interest}\n  {Persons[id - 1].Friends}\n";

        public string GetEventById(int Id) => $"{Events[Id-1].EventId} {Events[Id - 1].Type} creat:{Events[Id - 1].Creator} \n" +
            $"{Events[Id - 1].Title} {Events[Id - 1].Body} {Events[Id - 1].Sity}\n   {Events[Id - 1].Tegs}\n";

        #endregion


        public void AddEvent(Event e)
        {
            Events.Add(e);
        }

        public void AddPerson(Person p)
        {
            Persons.Add(p);
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
