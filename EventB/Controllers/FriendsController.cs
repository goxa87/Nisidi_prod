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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SearchFriend(string name, string teg, string city)
        {
            IEnumerable<User> users;
            var curUser = await context.Users.Include(e=>e.Friends).FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);
            // город
            if (!string.IsNullOrWhiteSpace(city))
            {
                city = city.Trim();
                users = context.Users.Where(e => e.City == city).ToList();
            }
            else
            {
                users = context.Users.Where(e => e.City == curUser.City).ToList();
            }
            // тег
            if (!string.IsNullOrWhiteSpace(teg))
            {
                var arr = tegSplitter.GetEnumerable(teg).ToArray();

                var usersWith = context.Intereses.Include(e => e.User).Where(e => e.Value == teg).Select(e => e.User).ToList();
                users = users.Intersect(usersWith);
            }
            // имя (вконце потомучто контейнс (выполняется в памяти))
            var usersRez = users;
            if (!string.IsNullOrWhiteSpace(name))
            {
                usersRez = usersRez.Where(e => e.Name.Contains(name)).ToList();
            }
            // Друзья пользователя.
            var isFriendFromSelect = usersRez.Join(curUser.Friends, rez => rez.Id, fr => fr.FriendUserId, (rez, fr) => rez).ToList();
            isFriendFromSelect.Add(curUser);
            // Вычесть друзей пользователя.
            usersRez = usersRez.Except(isFriendFromSelect).ToList();

            return View(usersRez);
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
                UserId = user.Id,
                FriendUserId = friendId,
                UserName = friend.Name,
                UserPhoto = friend.Photo,
                IsBlocked =false,
                IsConfirmed = false
            });
            // Обратный объект друга для оппонента
            await context.Friends.AddAsync(new Friend
            {
                UserId = friend.Id,
                FriendUserId = user.Id,
                UserName = user.Name,
                UserPhoto = user.Photo,
                IsBlocked = false,
                IsConfirmed = false
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