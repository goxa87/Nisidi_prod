using CommonServices.Infrastructure.WebApi;
using EventB.ViewModels.MarketRoom;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventB.Services.MarketKibnetApiServices
{
    public class MarketKibnetApiServices : IMarketKibnetApiServices
    {
        private readonly Context context;
        private readonly UserManager<User> userManager;
        IWebHostEnvironment environment;
        private readonly ILogger<MarketKibnetApiServices> _logger;

        /// <summary>
        /// Конфигурация приложения.
        /// </summary>
        private readonly IConfiguration _configuration;

        public MarketKibnetApiServices(Context _context,
            UserManager<User> _userManager,
             IWebHostEnvironment env,
             ILogger<MarketKibnetApiServices> logger,
             IConfiguration configuration)
        {
            context = _context;
            userManager = _userManager;
            environment = env;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Меняет тип  события на указанный.
        /// </summary>
        /// <param name="newType">тип события: 1 - частное, 0 - глобальное </param>
        /// <param name="eventId">id события</param>
        /// <param name="userName">имя текущего пользователя</param>
        /// <returns></returns>
        public async Task<bool> ChangeEventStatus(int newType, int eventId, string userId)
        {
            // Сдесь сделать логику оплаты для смены статуса. Пока не
            var curEvent = await context.Events.FirstOrDefaultAsync(e => e.EventId == eventId && e.UserId == userId);
            if (curEvent == null)
            {
                return false;
            }

            curEvent.Type =  (EventType)Enum.GetValues(typeof(EventType)).GetValue(newType); ;
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

        /// <inheritdoc />
        public async Task<WebResponce<List<SupportTicket>>> GetUserSupportTickets(string userId)
        {
            try
            {
                var tickets = await context.SupportTickets.Where(e => e.UserId == userId).OrderByDescending(e => e.OpenDate).ToListAsync();
                return new WebResponce<List<SupportTicket>>(tickets);
            }
            catch(Exception ex)
            {
                _logger.LogError($"ошибка GetUserSupportTickets. {ex.Message}\n{ex.StackTrace}");
                return new WebResponce<List<SupportTicket>>(null, false, ex.Message);
            }
        }

        /// <summary>
        /// блокировка пользователей в чатах
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="userChatId"></param>
        /// <param name="curentUserName"></param>
        /// <returns></returns>
        public async Task<int> SwitchUserChatBlock(int eventId, int userChatId, string curentUserId)
        {            
            var eve = await context.Events.Include(e=>e.Chat).ThenInclude(e=>e.UserChat).FirstOrDefaultAsync(e => e.EventId == eventId);
            if (eve == null)
                return 404;

            if(eve.UserId != curentUserId)
            {
                return 401;
            }
            var userChat = eve.Chat.UserChat.FirstOrDefault(e => e.UserChatId == userChatId);
            if (userChat == null)
                return 400;
            userChat.IsBlockedInChat = !userChat.IsBlockedInChat;
            await context.SaveChangesAsync();
            return 200;
        }

        /// <inheritdoc />
        public async Task<WebResponce<bool>> CreateNewSupportTicket(string message, string userId)
        {
            try
            {
                var newTicket = new SupportTicket()
                {
                    Description = message,
                    NeedToClose = true,
                    NisidiEmployeeId = _configuration.GetValue<string>("DefaultSupportEmployeeNisidiUserId"),
                    OpenDate = DateTime.Now,
                    Theme = message.Length > 20 ? message.Substring(0,20) : message,
                    UserId = userId
                };

                context.SupportTickets.Add(newTicket);
                await context.SaveChangesAsync();
                return new WebResponce<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateNewSupportTicket: {ex.Message} {ex.StackTrace}");
                return new WebResponce<bool>(false, false, "Ошибка создания обращения.");
            }            
        }

        /// <inheritdoc />
        public async Task<bool> DeleteEventBYOwner(int eventId, string userId)
        {
            var eve = await context.Events.Include(e => e.Chat)
               .Include(e => e.EventTegs)
               .Include(e => e.Vizits)
               .FirstOrDefaultAsync(e => e.EventId == eventId);
            if (eve.UserId != userId) return false;
            return await DeleteEvent(eve);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteEventByAdmin(int eventId, string token)
        {
            if (_configuration.GetValue<string>("DeleteEventToken") != token)
            {
                return false;
            }

            var eve = await context.Events.Include(e => e.Chat)
               .Include(e => e.EventTegs)
               .Include(e => e.Vizits)
               .FirstOrDefaultAsync(e => e.EventId == eventId);

            return await DeleteEvent(eve);
        }

        /// <summary>
        /// Удаление события.
        /// </summary>
        /// <param name="eventId">id события</param>
        /// <param name="userName">логин пользователя</param>
        /// <returns></returns>
        private async Task<bool> DeleteEvent(Event eve)
        {
            context.UserChats.RemoveRange(context.UserChats.Where(e => e.ChatId == eve.Chat.ChatId));
            context.Messages.RemoveRange(context.Messages.Where(e => e.ChatId == eve.Chat.ChatId));
            context.Chats.Remove(eve.Chat);

            // TODO Удалять все картинки
            var photoPath = environment.WebRootPath + "/" + eve.Image;

            if (File.Exists(photoPath))
            {
                File.Delete(photoPath);
            }

            photoPath = environment.WebRootPath + "/" + eve.MediumImage;

            if (File.Exists(photoPath))
            {
                File.Delete(photoPath);
            }

            photoPath = environment.WebRootPath + "/" + eve.MiniImage;

            if (File.Exists(photoPath))
            {
                File.Delete(photoPath);
            }

            context.Invites.RemoveRange(context.Invites.Where(e => e.EventId == eve.EventId));
            context.Remove(eve);

            await context.SaveChangesAsync();
            return true;
        }
    }
}
