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
    /// <summary>
    /// Этот контроллер передвет только вью. Форма выполняет из events 
    /// </summary>
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
        public async Task<IActionResult> Search(EventSearchViewModel model, int Skip = 0, int Take = 30)
        {
            // Добавить чтоб не было инъекций замены символов.
            // Узнать паттерны инъекций
           
            var user =await context.Users
                .Include(e=>e.Friends)
                .FirstOrDefaultAsync(e=>e.Name == User.Identity.Name);
            CostomSelectionArgs args = new CostomSelectionArgs {
                DateSince = model.DateStart,
                DateDue = model.DateEnd,
                IsTegsFromProfile = false,
                Title = model.Title.ToUpper(),
                City = model.City.ToUpper(),
                Tegs = model.Tegs.ToUpper()
            };

            var list = await eventSelector.GetCostomEventsAsync(args);
            return View(list);
        }

        [HttpGet]
        public IActionResult Search(IEnumerable<Event> list)
        {
            return View(list);
        }

        /// <summary>
        /// Вернет партиалку поиска
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetSearchPartial(EventSearchViewModel body)
        {
            return PartialView("~/Views/Shared/_SearchModalPartial.cshtml", body);
        }
    }
}