using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EventB.Services.MarketKibnetApiServices;
using EventB.ViewModels.MarketRoom;
using EventBLib.Models;
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
        private readonly IMarketKibnetApiServices roomApiServices;

        public MarketKibnetApiController(IMarketKibnetApiServices _kibnetApiServices)
        {
            roomApiServices = _kibnetApiServices;
        }
        [Route("change-event-type"), HttpGet]
        public async Task<StatusCodeResult> ChangeEventType(int targetType, int eventId)
        {
            if (targetType != 0)
            {
                ModelState.AddModelError("", "Недопустимое значение типа");
                return StatusCode(400);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await roomApiServices.ChangeEventStatus(targetType, eventId, userId);
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
            if (await roomApiServices.DeleteEventBYOwner(EventId, User.FindFirstValue(ClaimTypes.NameIdentifier)))
                return Ok();
            else
                return StatusCode(401);
        }

        /// <summary>
        /// Удаление события (администрация).
        /// </summary>
        /// <param name="EventId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("delete-event-admin"), HttpGet]
        public async Task<StatusCodeResult> DeleteEventAdmin(int eventId, string token)
        {
            if (await roomApiServices.DeleteEventByAdmin(eventId, token))
                return Ok();
            else
                return StatusCode(401);
        }

        /// <summary>
        /// Вернет список участников события
        /// </summary>
        /// <param name="EventId"></param>
        /// <returns></returns>
        [Route("get-event-chat-users"), HttpGet]
        public async Task<List<EventUserChatMembersVM>> GetEventUserChats(int EventId)
        {
            return await roomApiServices.GetEventUserChats(EventId);
        }
        /// <summary>
        /// блокировка пользователя в чате
        /// </summary>
        /// <param name="EventId"></param>
        /// <returns></returns>
        [Route("switch-user-chat-block"), HttpGet]
        public async Task<StatusCodeResult> SwitchUserChatBlock(int EventId, int userChatId)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return StatusCode(await roomApiServices.SwitchUserChatBlock(EventId, userChatId, userId));
        }
    }
}