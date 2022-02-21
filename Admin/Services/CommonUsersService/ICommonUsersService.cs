using Admin.Models.ViewModels.CommonUsers;
using CommonServices.Infrastructure.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services.CommonUsersService
{
    public interface ICommonUsersService
    {
        /// <summary>
        /// Получить список пользователей нисиди по параметрам
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<ListVM> GetUsersListByFilter(ListFilterParam param);

        /// <summary>
        /// Получить список пользователей нисиди по параметрам
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<UserDetailVM> GetUsersDetails(string userId);

        /// <summary>
        /// Подтвердить пользовательский имэйл
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WebResponce<bool>> ConfirnUserEmail(string userId);

        /// <summary>
        /// Переключить блокировку пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WebResponce<bool>> ToggleBlockUser(string userId);
    }
}
