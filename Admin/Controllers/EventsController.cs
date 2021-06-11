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

        public EventsController()
        {

        }

        public async Task<IActionResult> EventList()
        {
            return View();
        }
    }
}
