using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Authorization;
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
        
        public MyPageController(Context _context)
        {
            context = _context;
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
                ThenInclude(e=>e.Event).
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
    }
}