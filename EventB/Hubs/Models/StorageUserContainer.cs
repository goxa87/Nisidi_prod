using EventB.ViewModels.MessagesVM;
using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Hubs.Models
{
    /// <summary>
    /// Представляет 1 элемент 
    /// </summary>
    [Obsolete("Начал делать но потом передумапл . переделал на встроенные группы все")]
    public class StorageUserContainer
    {
        #region Свойства
        /// <summary>
        /// Имя Пользователя то которое содержится в контексте
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// ID пользователя из БД
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Массив соедиенений ассоциированных с данным пользователем
        /// </summary>
        public List<string> ConnectionsIds { get; set; }
        /// <summary>
        /// Буфер сообщений для пользователя, который находится на странице(чтоп не подгружать из БД на всяк случай)
        /// </summary>
        public List<ChatMessageVM> NewMessagesForUser { get; set; }
        #endregion

        public StorageUserContainer(User user, string connectionId)
        {
            UserId = user.Id;
            UserName = user.UserName;
            ConnectionsIds = new List<string>() { connectionId};
            NewMessagesForUser = new List<ChatMessageVM>();
        }
        
        /// <summary>
        /// Добавляет номер подключения
        /// </summary>
        /// <param name="conn"></param>
        public void AddConnection(string conn)
        {
            ConnectionsIds.Add(conn);
        }
        /// <summary>
        /// Удаляет номер подключения
        /// </summary>
        /// <param name="conn"></param>
        public void RemoveConnection(string conn)
        {
            ConnectionsIds.Remove(conn);
        }
        /// <summary>
        /// Все подключения пользователя
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAllUserConnections()
        {
            return this.ConnectionsIds;
        }
    }
}
