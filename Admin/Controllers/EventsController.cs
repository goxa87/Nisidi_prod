using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize]
    public class EventsController : AppControllerBase
    {

        public EventsController() : base()
        {

        }

        public async Task<IActionResult> EventList()
        {
            return View();
        }
    }
}
