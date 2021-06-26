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

        public async Task<IActionResult> EventList()
        {
            return View();
        }

        [HttpPost]
        [Route("Events/get-events-page")]
        public async Task<IActionResult> GetEventsPartialTablePage(EventListParam param = null)
        {
            if(param == null)
            {
                param = new EventListParam();
            }
            if(param.PagingParam == null)
            {
                param.PagingParam = new PagingParam();
                param.PagingParam.CurrentPage = 1;
                param.PagingParam.PageSize = 3;
            }
            var events = await eventsService.GetEventsList(param);
            return PartialView("~/Views/Events/_EventsTablePage.cshtml", events);
        }
    }
}
