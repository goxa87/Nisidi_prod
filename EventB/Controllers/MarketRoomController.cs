using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventB.ViewModels.MarketRoom;
using EventBLib.DataContext;
using EventBLib.Models.MarketingModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventB.Controllers
{
    public class MarketRoomController : Controller
    {
        readonly Context context;

        public MarketRoomController(Context _Context)
        {
            context = _Context;
        }

        public async Task<IActionResult> MarketRoom()
        {
            var user = await context.Users.Include(e=>e.MarketKibnet).Include(e=>e.MyEvents)
                .FirstOrDefaultAsync(e=>e.UserName == HttpContext.User.Identity.Name);

            var kibnet = await context.MarketKibnets.FirstOrDefaultAsync(e => e.UserId == user.Id);
            var model = new MarketRoomVM { 
                MarketKibnet = kibnet,
                Events = user.MyEvents,
                Banner = new List<MarketBanner>() 
            };

            return View(model);
        }
    }
}