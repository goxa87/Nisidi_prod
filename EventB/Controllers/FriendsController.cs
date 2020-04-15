using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventB.Services;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventB.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        readonly Context context;
        readonly IUserFindService userFind;
        public FriendsController(Context _context,
            IUserFindService _userFind)
        {
            context = _context;
            userFind = _userFind;
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
            var owner =await userFind.GetCurrentUserAsync(User.Identity.Name);

            List<Friend> friends = await context.Users.Where(e => e.Name.ToLower().Contains(search.ToLower())).
                Select(e => new Friend { CurrentUserId = owner.Id, UserName = e.Name, UserPhoto = e.Photo, UserId=e.Id}).
                ToListAsync();
            // удаляем себя как друга иначе будут коллизии.
            var ownerAsFriend = friends.FirstOrDefault(e => e.UserId == owner.Id);
            if (ownerAsFriend != null)
            {
                friends.Remove(ownerAsFriend);
            }
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
                CurrentUserId= user.Id,
                UserId=friendId,
                UserName = friend.Name,
                UserPhoto = friend.Photo
            });
            await context.SaveChangesAsync();
            RedirectToAction("List");
        }

        #region детали пользователя

        public async Task<IActionResult> UserInfo(string userId)
        {
            var user = await context.Users.FirstOrDefaultAsync(e => e.Id == userId);
            if (user == null)
            {
                Response.StatusCode = 204;
                return null;
            }

            return View(user);
        }
        #endregion
    }
}