using Admin.Models.ViewModels.CommonUsers;
using Admin.Services.CommonUsersService;
using Admin.Services.EventsService;
using CommonServices.Infrastructure.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize]
    public class CommonUsersController : Controller
    {
        /// <summary>
        /// Сервис пользователей нисиди
        /// </summary>
        private readonly ICommonUsersService _commonUsersService;

        /// <summary>
        /// Сервис событий
        /// </summary>
        private readonly IEventsService _eventsService;

        /// <summary>
        /// Конфигурация приложения.
        /// </summary>
        private readonly IConfiguration _configuration;

        public CommonUsersController(ICommonUsersService commonUsersService,
            IEventsService eventsService,
            IConfiguration configuration)
        {
            _commonUsersService = commonUsersService;
            _eventsService = eventsService;
            _configuration = configuration;
        }

        /// <summary>
        /// Список пользователей
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IActionResult> List(ListFilterParam filter = null)
        {
            return View(await _commonUsersService.GetUsersListByFilter(filter));
        }

        /// <summary>
        /// Детали пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Detail(string userId)
        {
            return View(await _commonUsersService.GetUsersDetails(userId));
        }

        #region API 

        /// <summary>
        /// Подтвердить пользовательский имэйл
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<WebResponce<bool>> ConfirmUserEmail(string userId)
        {
            return await _commonUsersService.ConfirnUserEmail(userId);
        }

        /// <summary>
        /// Переключить блокировку пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<WebResponce<bool>> ToggleBlockUser(string userId)
        {
            return await _commonUsersService.ToggleBlockUser(userId);
        }

        /// <summary>
        /// Пернет паршиалку со списком событий пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<PartialViewResult> GetCreatedEventsByuser(string userId)
        {
            var events = await _eventsService.GetEventListByUserId(userId, true);
            foreach (var eve in events)
            {
                eve.MediumImage = $"{_configuration.GetValue<string>("RootHostNisidi")}{eve.MediumImage}"; 
            }

            return PartialView("~/Views/Events/_EventsListCard.cshtml", events);
        }
        #endregion

    }
}
