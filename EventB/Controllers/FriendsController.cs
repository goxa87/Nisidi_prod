using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventB.Services;
using EventB.Services.FriendService;
using EventB.ViewModels.FriendsVM;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventB.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        readonly Context context;
        readonly IUserFindService userFind;
        readonly IFriendService friendService;
        readonly UserManager<User> userManager;
        public FriendsController(Context _context,
            IUserFindService _userFind,
            
            IFriendService _friendService,
            UserManager<User> _userManager)
        {
            context = _context;
            userFind = _userFind;
            friendService = _friendService;
            userManager = _userManager;
        }

        /// <summary>
        /// Начальная страница раздела. Выводит список друзей пользователя.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> List()
        {
            var user = await userFind.GetCurrentUserAsync(User.Identity.Name);
            var selection = await context.Friends.Where(e => e.FriendUserId == user.Id).ToListAsync();
            ViewBag.userId = user.Id;
            return View(selection);                        
        }
         
        /// <summary>
        /// Поиск пользователей. 
        /// </summary>
        /// <param name="search">Фрагмент имени пользователя.</param>
        /// <returns></returns>
        public async Task<ActionResult> SearchCurrentFriend(string search)
        {
            //Переделать
            //var owner =await userFind.GetCurrentUserAsync(User.Identity.Name);

            //List<Friend> friends = await context.Users.Where(e => e.Name.ToLower().Contains(search.ToLower())).
            //    Select(e => new Friend { UserId = owner.Id, UserName = e.Name, UserPhoto = e.Photo, FriendUserId = e.Id}).
            //    ToListAsync();
            //// удаляем себя как друга иначе будут коллизии.
            //var ownerAsFriend = friends.FirstOrDefault(e => e.UserId == owner.Id);
            //if (ownerAsFriend != null)
            //{
            //    friends.Remove(ownerAsFriend);
            //}
            return View(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="teg"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/Friends/SearchFriend")]
        public async Task<ActionResult> SearchFriend(string name, string teg, string city)
        {
            var usersResult = await friendService.SearchFriend(name, teg, city, User.Identity.Name);
            return View("SearchFriend", usersResult);
        }

        /// <summary>
        /// Это неактувльно делается на фронте.
        /// </summary>
        /// <param name="friendId"></param>
        /// <returns></returns>
        [HttpPost]
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
                UserId = user.Id,
                FriendUserId = friendId,
                UserName = user.Name,
                UserPhoto = user.Photo,
                IsBlocked =false,
                IsConfirmed = false, 
                FriendInitiator = false

            });
            // Обратный объект друга для оппонента
            await context.Friends.AddAsync(new Friend
            {
                UserId = friend.Id,
                FriendUserId = user.Id,
                UserName = friend.Name,
                UserPhoto = friend.Photo,
                IsBlocked = false,
                IsConfirmed = false,
                FriendInitiator = true
            });            

            await context.SaveChangesAsync();
            RedirectToAction("List");
        }

        #region детали пользователя
        /// <summary>
        /// Подробная страница пользователя.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <returns></returns>
        public async Task<IActionResult> UserInfo(string userId)
        {
            var currentUser = await userManager.FindByNameAsync(User.Identity.Name);
            if (currentUser.Id == userId) RedirectToAction("Index", "MyPage");
            var model = await friendService.GetFriendInfo(userId, currentUser);

            return View(model);
        }
        #endregion
    }
}