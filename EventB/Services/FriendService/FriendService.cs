using EventB.ViewModels.FriendsVM;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.FriendService
{
    public class FriendService : IFriendService
    {
        readonly IUserFindService userFind;
        readonly Context context;
        public FriendService(
            IUserFindService _userFind,
            Context _context
            )
        {
            userFind = _userFind;
            context = _context;
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
    }
}
