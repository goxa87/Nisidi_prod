using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EventB.Models;
using EventB.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EventB.Services;
using Microsoft.AspNetCore.Identity;
using EventB.Auth;
using EventB.Data;

namespace EventB.Controllers
{
    public class EventsController : Controller
    {
        private Context _context { get; }
        private IEventSelectorService selectorService { get; set; }
        private readonly SignInManager<User> userManager;
        private readonly IDataProvider data;

        public EventsController(Context c, IEventSelectorService ss , SignInManager<User> UM, IDataProvider db)
        {
            _context = c;
            selectorService = ss;
            userManager = UM;
            data = db;
        }

        public async Task<IActionResult> Start()
        {
            if (User.Identity.IsAuthenticated)
            {
                var person = _context.Persons.Where(e => e.Email == User.Identity.Name).First();
                return View(selectorService.GetStartEventList(person, data).ToList());
            }

            return View( _context.Events.Where(e=>e.Sity.ToLower()=="москва").Take(30));
            //return View(list);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(string Title,string Body, string Tegs, string Sity, string Place, DateTime Date)
        {
            _context.Events.Add(
                new Event() { 
                            Title=Title,
                            Body=Body,
                            Tegs=Tegs,
                            Sity= Sity,
                            Place= Place,
                            Date=Date,
                            Creator=_context.Persons.Where(e=>e.Email==User.Identity.Name).SingleOrDefaultAsync().Result.PersonId,
                            Image="/img/img.jpg",
                            CreationDate = DateTime.Now,
                            Likes=0,
                            Views=0,
                            Shares=0
                            }
                );
            await _context.SaveChangesAsync();
            return RedirectToAction("Start");
        }
    }
}