using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    public class AdminUsersController : Controller
    {


        public AdminUsersController()
        {

        }


        public async Task<IActionResult> List()
        {
            return View();
        }
    }
}
