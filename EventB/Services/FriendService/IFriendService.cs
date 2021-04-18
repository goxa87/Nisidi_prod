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
        Task<UserInfoVM> GetFriendInfo(string userId, User curentUser);
    }
}
