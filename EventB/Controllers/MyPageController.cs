using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBLib.DataContext;
using EventBLib.Models;
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
                Include(e => e.Intereses).
                Include(e => e.MyEvents).
                Include(e => e.Vizits).
                FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);

            var ev1 = context.Events.First(e => e.EventId == 1); 

            var invetes = new List<Invite>
            {
                new Invite{
                    EventId=1,
                    InviteId=1,
                    User=user,
                    InviterName="Name",
                    InviterPhoto="/images/Profileimages/26_04_20_47_5017032020me1.jpg",
                    InviteDescription="jgbcfybt jgbcfybt jgbctybt dscription/////////////////////////////////jgbcfybt jgbcfybt jgbctybt dscription/////////////////////////////////jgbcfybt jgbcfybt jgbctybt dscription/////////////////////////////////jgbcfybt jgbcfybt jgbctybt dscription/////////////////////////////////",
                    Event=ev1
                }
            };

            var friends = await context.Friends.Where(e => e.FriendUserId == user.Id && e.IsBlocked == false).ToListAsync();
            user.Friends = friends;
            user.Invites = invetes;
            return View(user);
        }
    }
}