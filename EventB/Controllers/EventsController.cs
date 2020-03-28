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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace EventB.Controllers
{
    public class EventsController : Controller
    {
        private Context context { get; }
        private readonly SignInManager<User> userManager;
        IWebHostEnvironment environment;
        IViewModelFactory _VMFactory;

        public EventsController(Context c, 
            SignInManager<User> UM, 
            IWebHostEnvironment env,
            IViewModelFactory vmFactory)
        {
            context = c;
            userManager = UM;
            environment = env;
            _VMFactory = vmFactory;
        }

        public async Task<IActionResult> Start()
        {
            if (User.Identity.IsAuthenticated)
            {
                //var person = context.Persons.Where(e => e.Email == User.Identity.Name).First();
                return View();
            }

            return View( context.Events.Where(e=>e.City.ToLower()=="ставрополь").Take(30));
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
        public async Task<IActionResult> Add(string Title, string Body, string Tegs, string Sity, string Place, DateTime Date, IFormFile file)
        {
            //Person creator = _context.Persons.Where(e => e.Email == User.Identity.Name).SingleOrDefaultAsync().Result;

            //string src = "/images/defaultimg.jpg";
            //if (file != null)
            //{
                
            //    string fileName = String.Concat(DateTime.Now.ToString("dd-MM-yy_hh-mm"), "_", creator.PersonId.ToString(),"_",file.FileName);
            //    src = String.Concat("/images/EventImages/",fileName);

            //    using (var FS = new FileStream(environment.WebRootPath + src, FileMode.Create))
            //    {
            //        await file.CopyToAsync(FS);
            //    }
            //}

            //_context.Events.Add(
            //    new Event() { 
            //                Title=Title,
            //                Body=Body,
            //                Tegs=Tegs,
            //                City= Sity,
            //                Place= Place,
            //                Date=Date,
            //                Creator=creator.Name,
            //                Image=src,
            //                CreationDate = DateTime.Now,
            //                Likes=0,
            //                Views=0,
            //                Shares=0
            //                }
            //    );
            //await _context.SaveChangesAsync();
            return RedirectToAction("Start");
        }

        /// <summary>
        /// вызов деталей event по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id != null)
            {
                //var item = _VMFactory.GetEventDetailsViewModel(userManager, id.Value, data);
                return View();
            }

            return NotFound();

        }
    }
}