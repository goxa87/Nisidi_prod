using EventB.ViewModels;
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
                if (await context.Friends.Where(e => e.UserId == currentUser.Id && e.FriendUserId == userId).AnyAsync())
                {
                    return 204;
                }
                var friend = await userFind.GetUserByIdAsync(userId);
                // Создание новых записей (прямой и обратный друг). 
                var userFriend = new Friend
                {
                    UserId = currentUser.Id,
                    FriendUserId = friend.Id,
                    UserName = friend.Name,
                    UserPhoto = friend.MediumImage,
                    FriendInitiator = true
                };
                var userFriendReverce = new Friend
                {
                    UserId = friend.Id,
                    FriendUserId = currentUser.Id,
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

        public async Task<UserInfoVM> GetFriendInfo(string userId, string curentUserId)
        {
            var user = await context.Users.
                Include(e => e.Intereses).
                Include(e=>e.Friends).
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

            var curentFriend = user.Friends.FirstOrDefault(e => e.FriendUserId == curentUserId);
            if (user.Visibility == AccountVisible.FriendsOnly && curentFriend == null)
            {
                return null;
            }

            if (curentFriend != null && curentFriend.IsBlocked)
                return null;

            /// тот что в друзьях у текущего пользователя
            var curentUserFriend = await context.Friends.FirstOrDefaultAsync(e => e.UserId == curentUserId && e.FriendUserId == userId);

            var infoVM = new UserInfoVM();
            infoVM.User = user;
            infoVM.CreatedEvents = user.MyEvents;
            infoVM.WillGoEvents = user.Vizits;
            infoVM.Friends = user.Friends;
            infoVM.Friend = curentUserFriend;
            if (curentUserFriend != null)
                infoVM.IsFriend = true;
            else
                infoVM.IsFriend = false;
            return infoVM;
        }

        public async Task<FriendsSearchVM> SearchFriend(string name, string teg, string city, string userId)
        {
            var result = context.Users.Include(e => e.Intereses).Where(e => e.Visibility == AccountVisible.Visible && e.Id != userId);

            if (!string.IsNullOrWhiteSpace(city))
            {
                result = result.Where(e => e.NormalizedCity == city.Trim().ToUpper());
            }

            var splittedTegs = tegSplitter.GetEnumerable(teg);
            if (splittedTegs != null)
            {
                result = result.Where(e => e.Intereses.Any(x => splittedTegs.Contains(x.Value)));
            }

            name = name?.Trim();
            if (!string.IsNullOrEmpty(name))
            {
                result = result.Where(e => EF.Functions.Like(e.NormalizedName, $"%{name.ToUpper()}%"));
            }

            var list = result.Select(e=> new SmallFigure()
            {
                Image = e.MediumImage,
                Link = $"/Friends/UserInfo?userId={e.Id}",
                Title = e.Name
            }).ToList();

            return new FriendsSearchVM()
            {
                Friends = list,
                SearchParam = new FriendSearchParam()
                {
                    Name = name,
                    City = city,
                    Teg = teg
                }
            };
        }

        public async Task<int> SubmitFriend(string friendId, string currentUserId)
        {
            var entitys = await context.Friends.Where(e =>(e.UserId == currentUserId && e.FriendUserId == friendId) || (e.UserId == friendId && e.FriendUserId == currentUserId)).ToListAsync();
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
