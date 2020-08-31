using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EventB.Controllers
{
    public class HelpController : Controller
    {      
        public IActionResult Manual()
        {
            return View();
        }
        public IActionResult FAQ()
        {
            return View();
        }
        public IActionResult Contacts()
        {
            return View();
        }
    }
}