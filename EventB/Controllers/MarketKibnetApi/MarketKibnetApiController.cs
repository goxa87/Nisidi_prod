using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventB.Services.MarketKibnetApiServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventB.Controllers.MarketKibnetApi
{
    [Route("api/MarketKibnet")]
    [ApiController]
    [Authorize]
    public class MarketKibnetApiController : ControllerBase
    {
        private readonly IMarketKibnetApiServices kibnetApiServices;

        public MarketKibnetApiController(IMarketKibnetApiServices _kibnetApiServices)
        {
            kibnetApiServices = _kibnetApiServices;
        }
        [Route("change-event-type"), HttpGet]
        public async Task<StatusCodeResult> ChangeEventType(int targetType, int eventId)
        {
            if (targetType != 0)
            {
                ModelState.AddModelError("", "Недопустимое значение типа");
                return StatusCode(400);
            }
            var result = await kibnetApiServices.ChangeEventStatus(targetType, eventId, User.Identity.Name);
            if (result) return Ok();
            else return StatusCode(400);
        }

        /// <summary>
        /// Удаление события.
        /// </summary>
        /// <param name="EventId"></param>
        /// <returns></returns>
        [Route("delete-event"), HttpGet]
        public async Task<StatusCodeResult> DeleteEvent(int EventId)
        {
            if (await kibnetApiServices.DeleteEvent(EventId, User.Identity.Name))
                return Ok();
            else
                return StatusCode(401);
        }
    }
}