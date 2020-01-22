using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EventB
{
    public class RepositoryEventList : IRepositoryEvent
    {
        public List<IEvent> Events { get; set; }

        public RepositoryEventList()
        {
            Events = new List<IEvent>();
            int i = 1;
            while (i < 21)
            {
                Events.Add(new Event()
                {
                    EventId = i,
                    AuthorID = 1,
                    Title = $"Title {i}",
                    Body = $"Body {i} Body {i} Body {i} ",
                    Date = DateTime.Now.AddSeconds(i),
                    Sity = "Sity 21",
                    Place = "home 1",
                    Tegs = $"teg{i % 2}&teg{i % 3}",
                    Likes = 0,
                    Shares = 0,
                    Views = 0
                });
                i++;
            } 
        }
        /// <summary>
        /// добавление записи
        /// </summary>
        /// <param name="ev">запись</param>
        /// <returns>true если удачно</returns>
        public bool Add(IEvent ev)
        {
            bool flag = true;

            try
            {
                Events.ToList().Add(ev);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + " Добавление в IRepositoryEvent.Add");
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// Удаление записи по id
        /// </summary>
        /// <param name="id"> id записи для удаления </param>
        /// <returns>true если удачно</returns>
        public bool RemoveEvent(int id)
        {
            bool flag = true;

            try
            {
                flag = Events.Remove(Events.Where(e => e.EventId == id).FirstOrDefault());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + " Удаление в IRepositoryEvent.Add");
                flag = false;
            }
            return flag;
        }
    }
}
