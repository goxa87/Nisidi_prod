using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    public class Account :AppControllerBase
    {
        public Account() : base()
        {

        }


        public IActionResult Login()
        {
            return View();
        }


        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
    }
}
