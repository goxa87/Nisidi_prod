﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    public class SupportController : Controller
    {

        public SupportController()
        {

        }

        public async Task<IActionResult> Tasks()
        {
            return View();
        }
    }
}
