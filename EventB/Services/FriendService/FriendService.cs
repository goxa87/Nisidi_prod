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

        public async Task<UserInfoVM> GetFriendInfo(string userId, User curentUser)
        {
            var user = await context.Users.
                Include(e => e.Intereses).
                Include(e => e.Friends).
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
            // Если показ только для друзей
            var friend = user.Friends.FirstOrDefault(e => e.FriendUserId == userId && e.UserId == curentUser.Id);
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
            infoVM.Friends = await context.Friends.Where(e => e.FriendUserId == userId).ToListAsync();
            if (friend != null)
                infoVM.IsFriend = true;
            else
                infoVM.IsFriend = false;
            return infoVM;
        }

        public async Task<List<SmallFigureFriendVM>> SearchFriend(string name, string teg, string city, string currentUserAppName)
        {
            var currentUser = await context.Users.Include(e => e.Friends).FirstAsync(e => e.UserName == currentUserAppName);
            IQueryable<User> users;
            // город
            if (!string.IsNullOrWhiteSpace(city))
            {
                city = city.Trim().ToUpper();
                users = context.Users.Where(e => e.NormalizedCity == city.ToUpper());
            }
            else
            {
                users = context.Users.Where(e => e.NormalizedCity == currentUser.NormalizedCity);
            }
            // тег
            if (!string.IsNullOrWhiteSpace(teg))
            {
                var arr = tegSplitter.GetEnumerable(teg.ToUpper()).ToArray();

                var usersWithTegs = context.Intereses.Include(e => e.User).Where(e => e.Value == teg).Select(e => e.User);
                users = users.Intersect(usersWithTegs);
            }
            // имя (из тех что уже выбраны)
            var usersRes = users;
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToUpper();
                usersRes = usersRes.Where(e => EF.Functions.Like(e.Name, $"%{name}%"));
            }
            var localUsersRes = await usersRes.Intersect(users).ToListAsync();

            // Вычесть друзей пользователя и себя из тех что попали в выборку.
            var isFriendFromSelect = localUsersRes.Join(currentUser.Friends, rez => rez.Id, fr => fr.FriendUserId, (rez, fr) => rez).ToList();
            isFriendFromSelect.Add(currentUser);

            var usersResult = localUsersRes.Except(isFriendFromSelect).Select(e => new SmallFigureFriendVM()
            {
                Image = e.Photo,
                Link = $"/Friends/UserInfo?userId={e.Id}",
                Title = e.Name
            }).ToList();
            return usersResult;
        }
    }
}
