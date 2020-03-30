using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventB.DataContext;
using EventB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventB.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        readonly Context context;

        public FriendsController(Context _context)
        {
            context = _context;
        }

        /// <summary>
        /// Начальная страница раздела. Выводит список друзей пользователя.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> List()
        {
            var selection = await context.Users.Where(e => e.UserName == User.Identity.Name).Include(e => e.Friends).FirstOrDefaultAsync();
            return View(selection.Friends);                        
        }
         
        /// <summary>
        /// Поиск пользователей. 
        /// </summary>
        /// <param name="search">Фрагмент имени пользователя.</param>
        /// <returns></returns>
        public async Task<ActionResult> SearchFriend(string search)
        {
            var owner = await context.Users.FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);

            List<Friend> friends = await context.Users.Where(e => e.Name.ToLower().Contains(search.ToLower())).
                Select(e => new Friend { UserId = owner.Id, UserName = e.Name, UserPhoto = e.Photo, PersonFriendId=e.Id}).
                ToListAsync();

            return View(friends);
        }

        [HttpPost]
        [Authorize]
        public async Task AddFriend(string friendId) 
        {
            // !!! исключить тех, кто уже является другом. как то?

            var friend = await context.Users.FirstOrDefaultAsync(e => e.Id == friendId);
            if (friend == null)
            {
                return;
            }
            var user = await context.Users.FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);
            
            await context.Friends.AddAsync(new Friend 
            {
                UserId= user.Id,
                PersonFriendId=friendId,
                UserName = friend.Name,
                UserPhoto = friend.Photo
            });
            await context.SaveChangesAsync();
            RedirectToAction("List");
        }
    }
}