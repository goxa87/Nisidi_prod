using Admin.Services.SupportService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    public class SupportController : Controller
    {
        /// <summary>
        /// Сервис заявок
        /// </summary>
        private readonly ISupportService _supportService;

        public SupportController(ISupportService supportService)
        {
            _supportService = supportService;
        }

        /// <summary>
        /// Вернет вьюху с заявками
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Tasks()
        {
            var tickets = await _supportService.GetAllTickets();
            return View(tickets);
        }
    }
}
