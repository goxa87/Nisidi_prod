using Admin.Models.ViewModels.Events;
using Admin.Services.EventsService;
using CommonServices.Infrastructure.Paging;
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
    public class EventsController : Controller
    {

        private readonly IEventsService _eventsService;
        private readonly IConfiguration _configuration;

        public EventsController(IEventsService eventsService,
            IConfiguration configuration)
        {
            this._eventsService = eventsService;
            _configuration = configuration;
        }

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
            return await _eventsService.BanEventByReason(eventId, message);
        }

        #endregion
    }
}
