﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBLib.DataContext;
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
        public async Task<IActionResult> Index()
        {
            var user = await context.Users.
                Include(e=>e.Intereses).
                Include(e=>e.MyEvents).
                Include(e=>e.Vizits).
                FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);

            var friends = await context.Friends.Where(e => e.FriendUserId == user.Id && e.IsBlocked == false).ToListAsync();
            user.Friends = friends;

            return View(user);
        }
    }
}