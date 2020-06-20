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
        public async Task<IActionResult> Search(EventSearchViewModel model, int Skip = 0, int Take = 12)
        {
            // Добавить чтоб не было инъекций замены символов.
            // Узнать паттерны инъекций
           
            var user =await context.Users
                .Include(e=>e.Friends)
                .FirstOrDefaultAsync(e=>e.Name == User.Identity.Name);
            CostomSelectionArgs args = new CostomSelectionArgs {
                DateSince = model.DateStart,
                DateDue = model.DateEnd,
                Title = model.Title.ToUpper(),
                City = model.Сity.ToUpper(),
                Tegs = model.Tegs.ToUpper()
            };

            var list = await eventSelector.GetCostomEventsAsync(args);

            await Task.Factory.StartNew(async ()=>
                {
                    foreach (var item in list)
                    {
                        item.Views++;
                    }
                    context.UpdateRange(list);
                    await context.SaveChangesAsync();
                });
            return View(list);
        }

        [HttpGet]
        public IActionResult Search(IEnumerable<Event> list)
        {
            return View(list);
        }
    }

    

}