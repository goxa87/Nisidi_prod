using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.AccountService
{
    /// <summary>
    /// Сервис для управления пользователем
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Удалить интерес пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task DeleteInteresForUser(string userId, string value);

        /// <summary>
        /// Добавить интерес пользователю
        /// </summary>
        /// <param name="user"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task AddInteresTouser(string userId, string value);
    }
}
