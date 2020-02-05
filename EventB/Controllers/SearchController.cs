using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventB.Data;
using EventB.Models;
using EventB.Services;
using EventB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventB.Controllers
{
    public class SearchController : Controller
    {

        readonly IEventSelectorService selector;
        readonly IDataProvider DB;

        public SearchController(IEventSelectorService s, IDataProvider db)
        {
            selector = s;
            DB = db;
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
        [HttpPost]
        public ActionResult Search(EventSearchViewModel model)
        {
            var param = new CostomSelectionArgs(model.DateStart, model.DateEnd, model.Title, model.Sity, model.Place, model.Tegs, -1);
            //var person = DB.GetPersons().Where(e => e.Email == User.Identity.Name);
            var rez = selector.GetCustomEventList(param, DB);
            
            return View(rez);
        }

        //public ActionResult SearchResult()
    }

    

}