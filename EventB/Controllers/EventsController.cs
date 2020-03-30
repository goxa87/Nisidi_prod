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
using EventB.ViewModels;

namespace EventB.Controllers
{
    public class EventsController : Controller
    {
        private Context context { get; }
        private readonly SignInManager<User> userManager;
        IWebHostEnvironment environment;

        public EventsController(Context c, 
            SignInManager<User> UM, 
            IWebHostEnvironment env)
        {
            context = c;
            userManager = UM;
            environment = env;
        }

        public async Task<IActionResult> Start()
        {
            if (User.Identity.IsAuthenticated)
            {

                return View( await context.Events.Where(e => e.City.ToLower() == "ставрополь").Take(30).ToListAsync());
            }

            return View(await context.Events.Where(e=>e.City.ToLower()=="ставрополь").Take(30).ToListAsync());
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
        public async Task<IActionResult> Add(AddEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Значение картинки если ее нет.
                string src = "/images/defaultimg.jpg";
                if (model.MainPicture != null)
                {
                    // Формирование строки имени картинки.
                    string fileName = String.Concat(DateTime.Now.ToString("dd-MM-yy_hh-mm"), "_", model.MainPicture.FileName);
                    src = String.Concat("/images/EventImages/", fileName);
                    // Запись на диск.
                    using (var FS = new FileStream(environment.WebRootPath + src, FileMode.Create))
                    {
                        await model.MainPicture.CopyToAsync(FS);
                    }
                }
                // Пользователь который выложил.
                var creator = await context.Users.FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);
                // Добавляем в посетителей автора.
                var vizit = new Vizit
                {
                    User = creator,
                    EventTitle = model.Title,
                    EventPhoto = src,
                    VizitorName = creator.Name,
                    VizitirPhoto = "/images/defaultimg.jpg"
                };
                
                var vizits = new List<Vizit> { vizit };
                // Итоговое формирование события.
                var eve = new Event
                {
                    Title = model.Title,
                    Body = model.Body,
                    Tegs = model.Tegs,
                    City = model.City,
                    Place = model.Place,
                    Date = model.Date,
                    Type = EventType.Private,
                    Likes = 0,
                    Views = 0,
                    Shares = 0,
                    WillGo = 1,
                    Creator = creator,
                    Image = src,
                    CreationDate = DateTime.Now,

                    Vizits = vizits
                };

                await context.Events.AddAsync(eve);
                await context.SaveChangesAsync();
                return RedirectToAction("Start");
            }
            else
            {
                ModelState.AddModelError("", "Неверные данные");
                return View(model);
            }            
        }

        /// <summary>
        /// вызов деталей event по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Details(int? id)
        {
            if (id != null)
            {
                var eve = await context.Events.
                    Include(e=>e.Creator).
                    Include(e=>e.Vizits).
                    FirstOrDefaultAsync(e => e.EventId == id);
                return View(eve);
            }

            return NotFound();
        }
    }
}