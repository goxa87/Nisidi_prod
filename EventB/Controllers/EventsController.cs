using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EventB.Models;
using EventB.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
//using EventB.Services;

namespace EventB.Controllers
{
    public class EventsController : Controller
    {
        private Context _context { get; }
        //private EventSelectorSevice 

        public EventsController(Context c)
        {
            _context = c;
        }

        public async Task<IActionResult> Start()
        {

            return View(await _context.Events.ToListAsync());
            //return View(list);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
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
                            Creator=0,
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