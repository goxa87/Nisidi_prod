using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventB.Services.MarketKibnetApiServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventB.Controllers.MarketKibnetApi
{
    [Route("api/MarketKibnet")]
    [ApiController]
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
            // Как для типов  enum EventType {Global 0, Private 1, Special недоступно}  

            if(!(targetType == 1 || targetType == 0))
            {
                ModelState.AddModelError("", "Недопустимое значение типа");
                return StatusCode(400);
            }
            var result = await kibnetApiServices.ChangeEventStatus(targetType, eventId, User.Identity.Name);
            if (result) return Ok();
            else return StatusCode(400);
        }
    }
}