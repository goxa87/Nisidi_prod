using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    public class CommonUsersController : Controller
    {
        public CommonUsersController()
        {

        }


        public async Task<IActionResult> List()
        {
            return View();
        } 

    }
}
