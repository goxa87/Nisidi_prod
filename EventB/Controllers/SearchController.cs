using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        SignInManager<User> signInManager;

        public SearchController( SignInManager<User> UM)
        {
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
            return View();
        }

        //public ActionResult SearchResult()
    }

    

}