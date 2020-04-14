using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBLib.Models;
using EventB.Services;
using EventB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EventBLib.DataContext;
using Microsoft.EntityFrameworkCore;

namespace EventB.Controllers
{
    public class SearchController : Controller
    {
        SignInManager<User> signInManager;
        Context context;
        IUserFindService findUser;
        IEventSelectorService eventSelector;

        public SearchController( SignInManager<User> UM,
            Context _context,
            IUserFindService _findUser,
            IEventSelectorService _eventSelector)
        {
            signInManager = UM;
            context = _context;
            findUser = _findUser;
            eventSelector = _eventSelector;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(EventSearchViewModel model)
        {
            var user =await context.Users.Include(e=>e.Friends).FirstOrDefaultAsync(e=>e.Name == User.Identity.Name);
            CostomSelectionArgs args = new CostomSelectionArgs(model.DateStart,
                model.DateEnd,
                model.Title,
                model.Сity,
                model.Tegs,
                model.FriendsOnly,
                user);

            var list = await eventSelector.GetCostomEventsAsync(args);
            return View(list);
        }

        [HttpGet]
        public IActionResult Search(IEnumerable<Event> list)
        {
            return View(list);
        }

        //public ActionResult SearchResult()
    }

    

}