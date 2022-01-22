using Admin.Models.Enums;
using Admin.Models.ViewModels.Support;
using Admin.Services.SupportService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

using CommonServices.Infrastructure.WebApi;
using System;
using Microsoft.Extensions.Logging;

namespace Admin.Controllers
{
    public class SupportController : Controller
    {
        /// <summary>
        /// Сервис заявок
        /// </summary>
        private readonly ISupportService _supportService;
        private readonly ILogger<SupportController> _logger;

        public SupportController(ISupportService supportService,
            ILogger<SupportController> logger)
        {
            _supportService = supportService;
            _logger = logger;
        }

        /// <summary>
        /// Вернет вьюху с заявками
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Tasks()
        {
            var tickets = await _supportService.GetTicketTickets(TicketFilterStatus.theNew, string.Empty);
            return View(tickets);
        }

        public async Task<IActionResult> GetPartialTaskListByType(int type)
        {
            var tickets = await _supportService.GetTicketTickets((TicketFilterStatus)type, User.FindFirstValue(ClaimTypes.NameIdentifier));
            return PartialView("_TasksList", tickets);
        }

        public async Task<IActionResult> TicketDetails(int ticketId)
        {
            var model = await _supportService.GetSupportTicketDetailsVM(ticketId);
            return View(model);
        }


        [HttpPost]
        public async Task<WebResponce<bool>> AssengreTicket(string ticketId)
        {
            try
            {
                var executionakResult = await _supportService.AssengreTicket(ticketId, User.FindFirstValue(ClaimTypes.NameIdentifier));
                if(executionakResult == true)
                {
                    return new WebResponce<bool>(true);
                }
                else
                {
                    return new WebResponce<bool>(false, false, "Метод вернул FALSE");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"{ex.Message}\n{ex.StackTrace}");
                return new WebResponce<bool>(false, false, ex.Message);
            }
        }

        [HttpPost]
        public async Task<WebResponce<bool>> SaveTicketDetails(string ticketId, string description, string note)
        {
            try
            {
                var executionakResult = await _supportService.SaveTicketDetails(ticketId, description, note, User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (executionakResult == true)
                {
                    return new WebResponce<bool>(true);
                }
                else
                {
                    return new WebResponce<bool>(false, false, "Метод вернул FALSE");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}\n{ex.StackTrace}");
                return new WebResponce<bool>(false, false, ex.Message);
            }
        }

        [HttpPost]
        public async Task<WebResponce<bool>> CloseTicket(string ticketId)
        {
            try
            {
                var executionakResult = await _supportService.CloseTicket(ticketId, User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (executionakResult == true)
                {
                    return new WebResponce<bool>(true);
                }
                else
                {
                    return new WebResponce<bool>(false, false, "Метод вернул FALSE");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}\n{ex.StackTrace}");
                return new WebResponce<bool>(false, false, ex.Message);
            }
        }
    }
}
