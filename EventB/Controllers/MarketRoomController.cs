using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EventB.ViewModels.MarketRoom;
using EventBLib.DataContext;
using EventBLib.Models.MarketingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventB.Controllers
{
    [Authorize]
    public class MarketRoomController : Controller
    {
        readonly Context context;

        public MarketRoomController(Context _Context)
        {
            context = _Context;
        }

        public async Task<IActionResult> MarketRoom()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var events = await context.Events.Include(e => e.Vizits).Include(e => e.EventTegs)
                .Where(e => e.UserId == userId).OrderByDescending(e => e.Date > DateTime.Now).ThenBy(e => e.Date).Take(300).ToListAsync();
           
            var model = new MarketRoomVM { 
                MarketKibnet = null,
                Events = events,
                Banner = new List<MarketBanner>() 
            };

            return View(model);
        }
        /// <summary>
        /// Форма оплаты 
        /// </summary>
        /// <returns></returns>
        public IActionResult Pay()
        {
            return View();
        }
    }
}