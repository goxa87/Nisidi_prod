using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EventB.Services;
using EventB.ViewModels.MyPage;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventB.Controllers
{
    /// <summary>
    /// Моя страница.
    /// </summary>
    [Authorize]
    public class MyPageController : Controller
    {
        readonly Context context;
        readonly UserManager<User> userManager;
        readonly ITegSplitter tegSplitter;
        readonly IWebHostEnvironment environment;
        public MyPageController(Context _context,
            UserManager<User> _userManager,
            ITegSplitter _tegSplitter,
            IWebHostEnvironment _environment)
        {
            context = _context;
            userManager = _userManager;
            tegSplitter = _tegSplitter;
            environment = _environment;
        }
        /// <summary>
        /// Страница профиля.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var user = await context.Users.
                Include(e => e.Intereses).
                Include(e => e.MyEvents).
                Include(e => e.Vizits).
                ThenInclude(e => e.Event).
                Include(e => e.Invites).
                ThenInclude(e => e.Event).
                FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);

            var friends = await context.Friends.Where(e => e.FriendUserId == user.Id && e.IsBlocked == false).ToListAsync();
            user.Friends = friends;
            return View(user);
        }

        /// <summary>
        /// Подтверждение приглашения.
        /// </summary>
        /// <param name="eventId">Ид события.</param>
        /// <param name="inviteId">Ид приглашения.</param>
        /// <returns></returns>
        [Route("/MyPage/SubmitInvite")]
        public async Task SubmitInvite(int eventId, int inviteId)
        {
            var curUser = await context.Users.FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);
            var eve = await context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            var inv = await context.Invites.FirstOrDefaultAsync(e => e.InviteId == inviteId);
            if (curUser == null || eve == null || inv == null)
            {
                Response.StatusCode = 410;
                return;
            }
            // Добавляем Визит.
            var newVizit = new Vizit
            {
                UserId = curUser.Id,
                EventId = eve.EventId,
                EventTitle = eve.TitleShort,
                EventPhoto = eve.Image,
                VizitorName = curUser.Name,
                VizitirPhoto = curUser.Photo
            };
            context.Vizits.Add(newVizit);
            // Удаляем приглашение.
            context.Invites.Remove(inv);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Отказ от приглашения.
        /// </summary>
        /// <param name="inviteId">Ид приглашения.</param>
        /// <returns></returns>
        [Route("/MyPage/RefuseInvite")]
        public async Task RefuseInvite(int inviteId)
        {
            var inv = await context.Invites.FirstOrDefaultAsync(e => e.InviteId == inviteId);
            if (inv == null)
            {
                Response.StatusCode = 410;
                return;
            }
            context.Invites.Remove(inv);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Заполнение формы для редактирования профиля.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var curUser = await userManager.GetUserAsync(HttpContext.User);
            var tegs = await context.Intereses.Where(e => e.UserId == curUser.Id).ToListAsync();
            var tegsStr = "";
            foreach (var teg in tegs)
            {
                tegsStr = $"{tegsStr} {teg.Value}";
            }
            var VM = new EditProfileVM
            {
                Name = curUser.Name,
                OldName = curUser.Name,
                OldPhoto = curUser.Photo,
                City = curUser.City,
                Tegs = tegsStr,
                OldTegs = tegsStr,
                Description = curUser.Description,
                AlowAnonMessages = curUser.AnonMessages,
                Visibility = curUser.Visibility,
                Phone = curUser.PhoneNumber
            };

            return View(VM);
        }
        /// <summary>
        /// Редактирование профиля.
        /// </summary>
        /// <param name="model">VM от пользователя.</param>
        /// <returns>Редирект к моей странице.</returns>
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileVM model)
        {
            if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.City))
            {
                ModelState.AddModelError("", "Заполните поля Имя и Город");
                return View(model);
            }
            else
            {
                var user = await userManager.GetUserAsync(HttpContext.User);
                
                List<Friend> inFriends=null;
                List<Invite> inInvites=null;
                List<Vizit> inVizits=null;
                if (model.newPhoto != null)
                {
                    var path = string.Concat("/images/Profileimages/", DateTime.Now.ToString("dd_MM_yy_mm_ss"), model.newPhoto.FileName)
                        .Replace(" ", "");
                    using (var FS = new FileStream(environment.WebRootPath + path, FileMode.Create))
                    {
                        await model.newPhoto.CopyToAsync(FS);
                    }
                    user.Photo = path;
                    // Изменить в друзьях, визиторах.
                    inFriends = await context.Friends.Where(e => e.UserId == user.Id).ToListAsync();
                    foreach (var e in inFriends)
                    {
                        e.UserPhoto = path;
                    }
                    inInvites = await context.Invites.Where(e => e.InviterId == user.Id).ToListAsync();
                    foreach (var e in inInvites)
                    {
                        e.InviterPhoto = path;
                    }
                    inVizits = await context.Vizits.Where(e => e.UserId == user.Id).ToListAsync();
                    foreach (var e in inVizits)
                    {
                        e.VizitirPhoto = path;
                    }
                    context.Friends.UpdateRange(inFriends);
                    context.Invites.UpdateRange(inInvites);
                    context.Vizits.UpdateRange(inVizits);
                }                
                // Изменения имени в таблицах.
                if (model.Name != model.OldName)
                {
                    user.Name = model.Name;
                    inFriends = inFriends != null ? inFriends : await context.Friends.Where(e => e.UserId == user.Id).ToListAsync();
                    foreach (var e in inFriends)
                    {
                        e.UserName = model.Name;
                    }
                    inInvites = inInvites != null ? inInvites : await context.Invites.Where(e => e.UserId == user.Id).ToListAsync();
                    foreach (var e in inInvites)
                    {
                        e.InviterName = model.Name;
                    }
                    inVizits = inVizits != null ? inVizits : await context.Vizits.Where(e => e.UserId == user.Id).ToListAsync();
                    foreach (var e in inVizits ?? await context.Vizits.Where(e => e.UserId == user.Id).ToListAsync())
                    {
                        e.VizitorName = model.Name;
                    }
                    context.Friends.UpdateRange(inFriends);
                    context.Invites.UpdateRange(inInvites);
                    context.Vizits.UpdateRange(inVizits);
                }
                // Изменение интересов.
                if (model.Tegs != model.OldTegs)
                {
                    var oldTegsDb = context.Intereses.Where(e => e.UserId == user.Id);
                    context.Intereses.RemoveRange(oldTegsDb);
                    var newTegs = tegSplitter.GetEnumerable(model.Tegs)
                        .Select(e => new Interes { Value = e })
                        .ToList();
                    user.Intereses = newTegs;
                }
                // Прочие изменения.
                user.City = model.City;
                user.Description = model.Description;
                user.AnonMessages = model.AlowAnonMessages;
                user.Visibility = model.Visibility;
                user.PhoneNumber = model.Phone;
                context.Users.Update(user);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }
    }
}