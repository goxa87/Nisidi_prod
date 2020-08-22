using EventBLib.Migrations;
using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Hubs.Models
{
    /// <summary>
    /// Класс для хранения подключений ассоциированных с определнными пользователями.
    /// </summary>
    [Obsolete("Начал делать но потом передумапл . переделал на встроенные группы все")]
    public class ConnectionStorage
    {
        #region Сойства
        /// <summary>
        /// Контейнеры пользователей
        /// </summary>
        List<StorageUserContainer> UsersContainers { get; set; }
        #endregion
        public ConnectionStorage()
        {
            UsersContainers = new List<StorageUserContainer>();
        }

        /// <summary>
        /// Добавляет нового пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <param name="connectionId"></param>
        public void AddUser(User user, string connectionId)
        {
            UsersContainers.Add(new StorageUserContainer(user, connectionId));
        }

        /// <summary>
        /// удаление пользователя
        /// </summary>
        /// <param name="userName"></param>
        public void RemoveUser(string userName)
        {
            var userContainer = UsersContainers.FirstOrDefault(e => e.UserName == userName);
            if (userContainer != null)
            {
                UsersContainers.Remove(userContainer);
            }
        }
        /// <summary>
        /// Добавить Коннекшн к пользователю
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="userName"></param>
        public void AddConnection(string connectionId, string userName)
        {
            try
            {
                var container = UsersContainers.SingleOrDefault(e => e.UserName == userName);
                if (container != null)
                {
                    container.AddConnection(connectionId);
                }
            }
            catch (Exception ex)
            {
                // логика объединения 2х одинаковых пользователей (предпологается что ошибка вылетает когда 57стр когда их 2 и больше с 1 имененм)
                // добавить в логгер
                Debug.WriteLine(ex);

                var sameUsers = UsersContainers.Where(e => e.UserName == userName).ToList();
                for(int i = 1;i<sameUsers.Count(); i++)
                {
                    sameUsers[0].ConnectionsIds.AddRange(sameUsers[i].ConnectionsIds);
                    RemoveUser(sameUsers[i].UserName);
                }
            }
            
        }
        /// <summary>
        /// Удаляет ConnectionId от контейнера (если последний то удаляет контейнер)
        /// </summary>
        /// <param name="connectionId"></param>
        public void RemoveConnection(string connectionId)
        {
            var userContainer = UsersContainers.FirstOrDefault(e => e.GetAllUserConnections().Any(y => y == connectionId));
            
            if (userContainer != null)
            {
                if(userContainer.GetAllUserConnections().Count()<1)
                {
                    UsersContainers.Remove(userContainer);
                    return;
                }    
                userContainer.RemoveConnection(connectionId);
            }
        }

        public IEnumerable<string> GetAllUsersIds()
        {
            return UsersContainers.Select(e => e.UserId);
        }

        // получение всех ConId ассоциированных с пользователем
        /// <summary>
        /// Вернет все строки подклчюения для пользователя по имени
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAllUserConnections(string userId)
        {
            var userContainer = UsersContainers.FirstOrDefault(e => e.UserId == userId);
            if(userContainer != null)
            {
                return userContainer.GetAllUserConnections();
            }
            return null;
        }

        public bool AnyUser(string userName)
        {
            return UsersContainers.Any(e => e.UserName == userName);
        }
    }
}
