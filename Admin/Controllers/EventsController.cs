using Admin.Models.ViewModels.Events;
using Admin.Services.EventsService;
using CommonServices.Infrastructure.Paging;
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

        private readonly IEventsService eventsService;
        private readonly IConfiguration _configuration;

        public EventsController(IEventsService _eventsService,
            IConfiguration configuration)
        {
            eventsService = _eventsService;
            _configuration = configuration;
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

        [HttpGet]
        public async Task<IActionResult> Details(int eventId)
        {
            var eve = await eventsService.GetEventDetails(eventId);
            return View("~/views/Events/Details.cshtml", eve);
        }

        [HttpPost]
        public async Task<ActionResult> GetGetEventsTable(EventListParam param)
        {
            var model = await eventsService.GetEventsList(param);
            return PartialView("~/views/Events/_EventsTablePage.cshtml", model.Events);
        }
    }
}
