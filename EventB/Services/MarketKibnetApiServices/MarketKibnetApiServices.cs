using EventB.ViewModels.MarketRoom;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.MarketKibnetApiServices
{
    public class MarketKibnetApiServices : IMarketKibnetApiServices
    {
        private readonly Context context;
        private readonly UserManager<User> userManager;
        IWebHostEnvironment environment;

        public MarketKibnetApiServices(Context _context,
            UserManager<User> _userManager,
             IWebHostEnvironment env)
        {
            context = _context;
            userManager = _userManager;
            environment = env;
        }

        /// <summary>
        /// Меняет тип  события на указанный.
        /// </summary>
        /// <param name="newType">тип события: 1 - частное, 0 - глобальное </param>
        /// <param name="eventId">id события</param>
        /// <param name="userName">имя текущего пользователя</param>
        /// <returns></returns>
        public async Task<bool> ChangeEventStatus(int newType, int eventId, string userName)
        {
            // Сдесь сделать логику оплаты для смены статуса. Пока не 
            var user = await userManager.FindByNameAsync(userName);
            var curEvent = await context.Events.FirstOrDefaultAsync(e => e.EventId == eventId && e.UserId == user.Id);
            if (curEvent == null)
            {
                return false;
            }

            curEvent.Type =  (EventType)Enum.GetValues(typeof(EventType)).GetValue(newType); ;
            await context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Удаление события.
        /// </summary>
        /// <param name="eventId">id события</param>
        /// <param name="userName">логин пользователя</param>
        /// <returns></returns>
        public async Task<bool> DeleteEvent(int eventId, string userName)
        {
            if (eventId == default || userName == default)
                return false;

            var user = await userManager.FindByNameAsync(userName);
            var eve = await context.Events.Include(e => e.Chat)
                .Include(e => e.EventTegs)
                .Include(e => e.Vizits)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eve.UserId != user.Id) return false;

            context.UserChats.RemoveRange(await context.UserChats.Where(e => e.ChatId == eve.Chat.ChatId).ToListAsync());
            context.Messages.RemoveRange(await context.Messages.Where(e => e.ChatId == eve.Chat.ChatId).ToListAsync());
            context.Chats.Remove(eve.Chat);

            // TODO Удалять все картинки
            var photoPath = environment.WebRootPath + "/" + eve.Image;
            Debug.WriteLine("image path: ", photoPath);

            if (File.Exists(photoPath))
            {
                File.Delete(photoPath);
            }

            context.Invites.RemoveRange(context.Invites.Where(e => e.EventId == eventId));
            context.Remove(eve);

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<EventUserChatMembersVM>> GetEventUserChats(int eventId)
        {
            var eve = await context.Events.Include(e => e.Chat).ThenInclude(e => e.UserChat).ThenInclude(e => e.User).FirstOrDefaultAsync(e => e.EventId == eventId);
            var chatMembers = eve.Chat.UserChat.Select(e => new EventB.ViewModels.MarketRoom.EventUserChatMembersVM() 
                {
                    UserId = e.UserId,
                    UserName = e.User.Name,
                    UserPhoto = e.User.MediumImage,
                    UserChatId = e.UserChatId,
                    IsBlocked = e.IsBlockedInChat
                }).ToList();
            return chatMembers;
        }

        /// <summary>
        /// блокировка пользователей в чатах
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="userChatId"></param>
        /// <param name="curentUserName"></param>
        /// <returns></returns>
        public async Task<int> SwitchUserChatBlock(int eventId, int userChatId, string curentUserName)
        {
            var user = await userManager.FindByNameAsync(curentUserName);
            var eve = await context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            if (eve == null)
                return 404;

            if(eve.UserId != user.Id)
            {
                return 401;
            }
            var userChat = await context.UserChats.FirstOrDefaultAsync(e => e.UserChatId == userChatId);
            if (userChat == null)
                return 400;
            userChat.IsBlockedInChat = !userChat.IsBlockedInChat;
            await context.SaveChangesAsync();
            return 200;
        }
    }
}
