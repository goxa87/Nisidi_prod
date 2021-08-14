using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EventB.Services;
using EventB.Services.FriendService;
using EventB.ViewModels.FriendsVM;
using EventB.ViewModels.SharedViews;
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
        /// Добавит пользователся в друзья
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<StatusCodeResult> AddFriend(string userId)
        {
            var currentUser = await userManager.FindByNameAsync(User.Identity.Name);
            var result = await friendService.AddUserAsFriend(userId, currentUser);
            return StatusCode(result);
        }

        /// <summary>
        /// Начальная страница раздела. Выводит список друзей пользователя.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> List()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var selection = await context.Friends.Where(e => e.UserId == userId).ToListAsync();
            ViewBag.userId = userId;
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

        /// <summary>
        /// Подтвердит заявку в друзья
        /// </summary>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public async Task<StatusCodeResult> SubmitFriend(string friendId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await friendService.SubmitFriend(friendId, currentUserId);
            return StatusCode(result);
        }
        #region детали пользователя
        /// <summary>
        /// Подробная страница пользователя.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <returns></returns>
        public async Task<IActionResult> UserInfo(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == userId) RedirectToAction("Index", "MyPage");
            var model = await friendService.GetFriendInfo(userId, currentUserId);

            if(model == null)
            {
                return View("~/Views/Shared/_AccessDeniedPage.cshtml", new AccessDeniedVM()
                {
                    Tittle = "Пользователь не найден",
                    Reazon = "Пользователь не найден или заблокировал переход на его страницу. ",
                    Text = "Попробуйте изменить параметры поиска, проверить правильность ввода адреса или разблокировать пользователя на вкладке ДРУЗЬЯ."
                });
            }

            return View(model);
        }
        #endregion
    }
}