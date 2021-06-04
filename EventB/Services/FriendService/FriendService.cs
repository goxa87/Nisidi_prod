using EventB.ViewModels.FriendsVM;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.FriendService
{
    public class FriendService : IFriendService
    {
        
        readonly Context context;
        readonly ITegSplitter tegSplitter;
        readonly IUserFindService userFind;
        readonly UserManager<User> userManager;
        public FriendService(
            IUserFindService _userFind,
            Context _context,
            ITegSplitter _tegSplitter,
            UserManager<User> _userManager
            )
        {
            userFind = _userFind;
            context = _context;
            tegSplitter = _tegSplitter;
            userManager = _userManager;
        }

        public async Task<int> AddUserAsFriend(string userId, User currentUser)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
               
                // Если есть такая запись о друзьях уже есть просто возвратить статус.
                if (context.Friends.Where(e => e.UserId == userId && e.FriendUserId == currentUser.Id).Any())
                {
                    return 204;
                }
                var friend = await userFind.GetUserByIdAsync(userId);
                // Создание новых записей (прямой и обратный друг). 
                var userFriend = new Friend
                {
                    UserId = friend.Id,
                    FriendUserId = currentUser.Id,
                    UserName = friend.Name,
                    UserPhoto = friend.MediumImage,
                    FriendInitiator = true
                };
                var userFriendReverce = new Friend
                {
                    UserId = currentUser.Id,
                    FriendUserId = friend.Id,
                    UserName = currentUser.Name,
                    UserPhoto = currentUser.MediumImage
                };

                await context.Friends.AddAsync(userFriend);
                await context.Friends.AddAsync(userFriendReverce);
                await context.SaveChangesAsync();

                return 200;
            }
            else
            {
                return 404;
            }
        }

        public async Task<UserInfoVM> GetFriendInfo(string userId, User curentUser)
        {
            var user = await context.Users.
                Include(e => e.Intereses).
                Include(e=>e.MyEvents).
                Include(e=>e.Vizits).ThenInclude(e=>e.Event).
                FirstOrDefaultAsync(e => e.Id == userId);
            if (user == null)
            {
                return null;
            }

            // Если пользователь заблокировал показ всем
            if (user.Visibility == AccountVisible.Unvisible)
            {
                return null;
            }
            // Если показ только для друзей (Выберем 1 раз)
            var allRelatedVisits = await context.Friends.Where(e => 
                (e.FriendUserId == curentUser.Id && e.UserId == user.Id) 
                || (e.FriendUserId == user.Id && e.UserId == curentUser.Id)
                || (e.FriendUserId == userId)).ToListAsync();
            var friend = allRelatedVisits.FirstOrDefault(e => e.FriendUserId == curentUser.Id && e.UserId == user.Id);
            if (user.Visibility == AccountVisible.FriendsOnly && friend == null)
            {
                return null;
            }
            if (friend != null && friend.IsBlocked && friend.BlockInitiator)
                return null;

            var infoVM = new UserInfoVM();
            infoVM.User = user;
            infoVM.CreatedEvents = user.MyEvents;
            infoVM.WillGoEvents = user.Vizits;
            infoVM.Friends = allRelatedVisits.Where(e => e.FriendUserId == userId).ToList();
            infoVM.Friend = friend;
            if (friend != null)
                infoVM.IsFriend = true;
            else
                infoVM.IsFriend = false;
            return infoVM;
        }

        public async Task<List<SmallFigureFriendVM>> SearchFriend(string name, string teg, string city, string currentUserAppName)
        {
            var currentUser = await userManager.FindByNameAsync(currentUserAppName);

            city = city?.Trim().ToUpper() ?? currentUser.NormalizedCity;
            var result = context.Users.Include(e => e.Intereses).Where(e => e.NormalizedCity == city
                  && e.Visibility == AccountVisible.Visible
                  && e.UserName != currentUserAppName);
            
            var splittedTegs = tegSplitter.GetEnumerable(teg);
            if (splittedTegs != null)
            {
                result = result.Where(e => e.Intereses.Any(x => splittedTegs.Contains(x.Value)));
            }

            name = name?.Trim();
            if (!string.IsNullOrWhiteSpace(name))
            {
                result = result.Where(e => EF.Functions.Like(e.Name, $"%{name}%"));
            }

            return result.Select(e=> new SmallFigureFriendVM()
            {
                Image = e.MediumImage,
                Link = $"/Friends/UserInfo?userId={e.Id}",
                Title = e.Name
            }).ToList();
        }

        public async Task<int> SubmitFriend(string friendId, User currentUser)
        {
            var entitys = await context.Friends.Where(e =>(e.UserId == friendId && e.FriendUserId == currentUser.Id) || (e.UserId == currentUser.Id && e.FriendUserId == friendId)).ToListAsync();
            if (entitys.Count == 0)
            {
                return 400;
            }
            foreach(var e in entitys)
            {
                e.IsConfirmed = true;
            }
            await context.SaveChangesAsync();
            return 200;
        }
    }
}
