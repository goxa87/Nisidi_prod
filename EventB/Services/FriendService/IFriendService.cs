using EventB.ViewModels;
using EventB.ViewModels.FriendsVM;
using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.FriendService
{
    public interface IFriendService
    {
        /// <summary>
        /// Получить модель для представления в деталях друга
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="curentUser">Текущий пользователь</param>
        /// <returns></returns>
        Task<UserInfoVM> GetFriendInfo(string userId, string curentUserId);

        /// <summary>
        /// Получит список потенциальных друзей
        /// </summary>
        /// <returns></returns>
        Task<FriendsSearchVM> SearchFriend(string name, string teg, string city, string userId);

        /// <summary>
        /// Добавит пользователя в друзья
        /// </summary>
        /// <param name="userId">ИД того кого добавляем</param>
        /// <param name="curentUser">текущий пользователь</param>
        /// <returns></returns>
        Task<int> AddUserAsFriend(string userId, User curentUser);

        /// <summary>
        /// Подтвердит друга для пользователя
        /// </summary>
        /// <param name="friendId">ИД друга</param>
        /// <param name="currentUser">текущий пользователь</param>
        /// <returns></returns>
        Task<int> SubmitFriend(string friendId, string currentUserId);
    }
}
