using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EventB.Controllers
{
    public class EventsController : Controller
    {
        RepositoryEventList list { get; set; } = new RepositoryEventList();

        public IActionResult Start()
        {           
            return View(list);
        }



        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Event ev)
        {
            var ev2 = new Event() { Body = ev.Body, Title = ev.Title };
            list.Add(ev2);
            return RedirectToAction("Start", "Events");
        }
    }
}