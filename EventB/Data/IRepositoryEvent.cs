using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB
{
    /// <summary>
    ///  Представляет репозиторий хранения записей IEvent
    /// </summary>
    public interface IRepositoryEvent
    {
        /// <summary>
        /// Коллекция Событий
        /// </summary>
        public List<IEvent> Events { get; set; }
        /// <summary>
        /// Добавление в коллекцию записи
        /// </summary>
        /// <param name="ev"></param>
        /// <returns></returns>
        public bool Add(IEvent ev);
        /// <summary>
        /// удаление записи по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool RemoveEvent(int id);
    }
}
