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
        /// Поиск друзей
        /// </summary>
        /// <param name="name">Часть имени</param>
        /// <param name="teg">интересы пользователя полностью</param>
        /// <param name="city">город(по-умолчанию тот что в настройках профиля того что ищет)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("/Friends/SearchFriend")]
        public async Task<ActionResult> SearchFriend(string name, string teg, string city)
        {
            var usersResult = await friendService.SearchFriend(name, teg, city, User.Identity.Name);
            return View("SearchFriend", usersResult);
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