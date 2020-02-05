using EventB.Data;
using EventB.DataContext;
using EventB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services
{
    /// <summary>
    /// представляет выборку экземпляров собятия
    /// </summary>
    public interface IEventSelectorService
    {
        /// <summary>
        /// Метод возвращает результат поиска по-умолчанию для текущего пользователя
        /// </summary>
        /// <param name="p">пользователь для которого будет осуществлятся поиск</param>
        /// <param name="context">сонтекст данных</param>
        /// <returns>список событий удовлетворяющий условиям</returns>
        public IEnumerable<Event> GetStartEventList(Person p, IDataProvider context);

        /// <summary>
        /// Метод возвращающий результат настраиваемого поиска событий
        /// </summary>
        /// <param name="args">Аргументы расширенного поиска</param>
        /// <param name="p">пользователь для которого осуществляется потиск</param>
        /// <param name="context">сонтекст данных</param>
        /// <returns>список событий удовлетворяющий условиям</returns>
        public IEnumerable<Event> GetCustomEventList(CostomSelectionArgs args, IDataProvider db);
    }
}
