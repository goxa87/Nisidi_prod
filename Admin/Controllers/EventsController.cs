using Admin.Models.ViewModels.Events;
using Admin.Services.EventsService;
using CommonServices.Infrastructure.Paging;
using CommonServices.Infrastructure.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {

        private readonly IEventsService _eventsService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventsService eventsService,
            IConfiguration configuration,
            ILogger<EventsController> logger)
        {
            this._eventsService = eventsService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// При 1й загрузке страницы
        /// </summary>
        /// <param name="eventsListParam"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public async Task<IActionResult> EventList(EventListParam eventsListParam = null)
        {
            if(eventsListParam == null)
            {
                eventsListParam = new EventListParam();
                eventsListParam.OnlyRequereCheck = true;
            }

            eventsListParam.OnlyRequereCheck = true;
            var model = await _eventsService.GetEventsList(eventsListParam);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int eventId)
        {
            var eve = await _eventsService.GetEventDetails(eventId);
            return View("~/views/Events/Details.cshtml", eve);
        }

        /// <summary>
        /// Загрузка паршиалки фильтром
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetEventsTable(EventListParam param)
        {
            var model = await _eventsService.GetEventsList(param);
            return PartialView("~/views/Events/_EventsTablePage.cshtml", model.Events);
        }

        #region управление событиями

        /// <summary>
        /// Одобрить событие
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task<WebResponce<string>> ConfirmEvent(int eventId)
        {
            return await _eventsService.ConfirmEventStatus(eventId);
        }

        /// <summary>
        /// Заблокировать событие
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<WebResponce<string>> BanEvent(int eventId, string message)
        {
            return await _eventsService.BanEventByReason(eventId, message, User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        /// <summary>
        /// Удалить событие безвозвратно
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<WebResponce<bool>> DeleteEvent(int eventId)
        {
            try
            {
                var result = await _eventsService.DeleteEvent(eventId, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Не удалось удалить событие {eventId}.");
                return new WebResponce<bool>(false, false, ex.Message);
            }
        }
        #endregion
    }
}
