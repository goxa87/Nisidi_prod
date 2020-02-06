using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventB.Auth;
using EventB.Data;
using EventB.Models;
using EventB.Services;
using EventB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventB.Controllers
{
    public class SearchController : Controller
    {

        readonly IEventSelectorService selector;
        readonly IDataProvider DB;
        SignInManager<User> signInManager;

        public SearchController(IEventSelectorService s, IDataProvider db, SignInManager<User> UM)
        {
            selector = s;
            DB = db;
            signInManager = UM;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public IActionResult Search(IEnumerable<Event> list)
        //{
        //    return View(list);
        //}

        [AllowAnonymous]
        [HttpPost,HttpGet]
        public ActionResult Search(EventSearchViewModel model)
        {
            Person requester = null;
            // выбор пользователя
            if (User.Identity.IsAuthenticated)
                requester = DB.GetPersons().Where(e => e.Email == User.Identity.Name).First();

            var param = new CostomSelectionArgs
                (model.DateStart, model.DateEnd, model.Title, model.Sity, model.Place, model.Tegs, -1, model.FriendsOnly,requester);
            //var person = DB.GetPersons().Where(e => e.Email == User.Identity.Name);
            var rez = selector.GetCustomEventList(param, DB);
            
            return View(rez);
        }

        //public ActionResult SearchResult()
    }

    

}