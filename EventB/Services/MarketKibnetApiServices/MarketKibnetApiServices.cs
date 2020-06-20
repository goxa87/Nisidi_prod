using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.MarketKibnetApiServices
{
    public class MarketKibnetApiServices : IMarketKibnetApiServices
    {
        private readonly Context context;
        private readonly UserManager<User> userManager;

        public MarketKibnetApiServices(Context _context,
            UserManager<User> _userManager)
        {
            context = _context;
            userManager = _userManager;
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
    }
}
