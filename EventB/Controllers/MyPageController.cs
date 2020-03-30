using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventB.DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventB.Controllers
{
    public class MyPageController : Controller
    {
        readonly Context context;
        public MyPageController(Context _context)
        {
            context = _context;
        }

        [Authorize]
        public IActionResult Index()
        {
            var user = context.Users.
                Include(e=>e.Intereses).
                Include(e=>e.MyEvents).
                Include(e=>e.Vizits).
                FirstOrDefault(e => e.UserName == User.Identity.Name);

            return View(user);
        }
    }
}