using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventB.Services;
using EventB.ViewModels.FriendsVM;
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
        readonly ITegSplitter tegSplitter;
        public FriendsController(Context _context,
            IUserFindService _userFind,
            ITegSplitter _tegSplitter)
        {
            context = _context;
            userFind = _userFind;
            tegSplitter = _tegSplitter;
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
        // это похоже решено на стороне клиента
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="teg"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("/Friends/SearchFriend")]
        public async Task<PartialViewResult> SearchFriend(string name, string teg, string city)
        {
            IEnumerable<User> users;
            var curUser = await context.Users.Include(e=>e.Friends).FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);
            // город
            if (!string.IsNullOrWhiteSpace(city))
            {
                city = city.Trim().ToUpper();
                users = context.Users.Where(e => e.NormalizedCity == city.ToUpper()).ToList();
            }
            else
            {
                users = context.Users.Where(e => e.NormalizedCity == curUser.NormalizedCity).ToList();
            }
            // тег
            if (!string.IsNullOrWhiteSpace(teg))
            {
                var arr = tegSplitter.GetEnumerable(teg.ToUpper()).ToArray();

                var usersWith = context.Intereses.Include(e => e.User).Where(e => e.Value == teg).Select(e => e.User).ToList();
                users = users.Intersect(usersWith);
            }
            // имя (вконце потомучто контейнс (выполняется в памяти))
            var usersRez = users;
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToUpper();
                usersRez = usersRez.Where(e => e.NormalizedName.Contains(name)).ToList();
            }
            // Друзья пользователя.
            var isFriendFromSelect = usersRez.Join(curUser.Friends, rez => rez.Id, fr => fr.FriendUserId, (rez, fr) => rez).ToList();
            isFriendFromSelect.Add(curUser);
            // Вычесть друзей пользователя и себя.
            var usersResult = usersRez.Except(isFriendFromSelect).Select(e => new SmallFigureFriendVM() {
                Image = e.Photo,
                Link = $"/Friends/UserInfo?userId={e.Id}",
                Title = e.Name
            }).ToList();

            return PartialView("_friendListSmallPartial", usersResult);
        }
        /// <summary>
        /// Это неактувльно делается на фронте.
        /// </summary>
        /// <param name="friendId"></param>
        /// <returns></returns>
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
            var currUser = await userFind.GetCurrentUserAsync(User.Identity.Name);
            var user = await context.Users.
                Include(e=>e.Intereses).
                Include(e=>e.Friends).
                FirstOrDefaultAsync(e => e.Id == userId);
            if (user == null)
            {
                Response.StatusCode = 400;
                return View(null);
            }
            // Если это собственно сам, то редирект на мою страницу.
            if (currUser == user)
            {
                return RedirectToAction("Index", "MyPage");
            }

            // Если пользователь заблокировал показ всем
            if (user.Visibility == AccountVisible.Unvisible)
            {
                return View(null);
            }
            // Если показ только для друзей
            var friend = await context.Friends.FirstOrDefaultAsync(e => e.FriendUserId == userId && e.UserId == currUser.Id);
            if (user.Visibility == AccountVisible.FriendsOnly && friend == null)
            {
                return View(null);
            }
            if (friend!=null && friend.IsBlocked && friend.BlockInitiator)
                return View(null);
            
            // Формирование VM.
            var createdEve = await context.Events.Where(e => e.UserId == userId).ToListAsync();
            var vizitEve = await context.Vizits.Where(e => e.UserId == userId).ToListAsync();
            var friends = await context.Friends.Where(e => e.FriendUserId == userId).ToListAsync();
            var asFriend = await context.Friends.FirstOrDefaultAsync(e => e.FriendUserId == currUser.Id && e.UserId == userId);
            var infoVM = new UserInfoVM();
            infoVM.User = user;
            infoVM.Friend = asFriend;
            infoVM.CreatedEvents = createdEve;
            infoVM.WillGoEvents = vizitEve;
            infoVM.Friends = friends;
            if (friend != null)
                infoVM.IsFriend = true;
            else
                infoVM.IsFriend = false;

            // Подставить вычисление.
            
            return View(infoVM);
        }
        #endregion
    }
}