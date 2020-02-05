using EventB.DataContext;
using EventB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Data
{

    public class DbData : IDataProvider
    {
        //экземпляр контекста
        public Context context { get; set; }

        public DbData(Context c) => context = c;
        /// <summary>
        /// добавить Событие асинхронно
        /// </summary>
        /// <param name="e">экземпляр события</param>
        public async void AddEvent(Event e)
        {
            context.Events.Add(e);
            await context.SaveChangesAsync();
        }
        /// <summary>
        /// добавить в БД Person асинхрнонно
        /// </summary>
        /// <param name="p">Пользователь</param>
        public async void AddPerson(Person p)
        {
            context.Persons.Add(p);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Добавляет информацию о посещении события пользователем
        /// </summary>
        /// <param name="v">экземпляр Vizitors который связывает событтия и пользователей</param>
        public async void AddVizitor(Vizitors v)
        {
            context.Vizitors.Add(v);
            await context.SaveChangesAsync();
        }

        public IEnumerable<Chat> GetChat() => context.Chats;

        public IEnumerable<Event> GetEvents() => context.Events;

        public IEnumerable<Message> GetMessage() => context.Messages;

        public IEnumerable<Person> GetPersons() => context.Persons;

        public IEnumerable<UserChat> GetUserChat() =>context.UserChats;

        public IEnumerable<Vizitors> GetVizitors() => context.Vizitors;
    }
}
