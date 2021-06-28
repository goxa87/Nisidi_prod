using Admin.Models.ViewModels.Events;
using Admin.Services.EventsService;
using CommonServices.Infrastructure.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {

        private readonly IEventsService eventsService;

        public EventsController(IEventsService _eventsService)
        {
            eventsService = _eventsService;
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> EventList(EventListParam eventsListParam = null)
        {
            if(eventsListParam == null)
            {
                eventsListParam = new EventListParam();
            }
            var model = await eventsService.GetEventsList(eventsListParam);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> GetGetEventsTable(EventListParam param)
        {
            var model = await eventsService.GetEventsList(param);
            return PartialView("_EventsTablePage.cshtml", model.Events);
        }
    }
}
