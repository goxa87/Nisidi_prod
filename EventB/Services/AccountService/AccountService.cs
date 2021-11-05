using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.AccountService
{
    /// <summary>
    /// Сервис Управления пользователем
    /// </summary>
    public class AccountService : IAccountService
    {
        /// <summary>
        /// Контекст ДБ
        /// </summary>
        private readonly Context context;


        private readonly ITegSplitter tegSplitter;

        /// <summary>
        /// Новый сервис пользователей
        /// </summary>
        public AccountService(Context _context,
            ITegSplitter _tegSplitter)
        {
            context = _context;
            tegSplitter = _tegSplitter;
        }

        /// <inheritdoc />
        public async Task AddInteresTouser(string userId, string value)
        {
            var user = await context.Users.Include(e=>e.Intereses).FirstAsync(e => e.Id == userId);
            foreach (var newInteres in tegSplitter.GetEnumerable(value))
            {
                if(user.Intereses.Any(e=>e.Value == value))
                {
                    continue;
                }
                user.Intereses.Add(new Interes()
                {
                    Value = newInteres
                });
            }
            await context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteInteresForUser(string userId, string value)
        {
            var user = await context.Users.Include(e => e.Intereses).FirstAsync(e => e.Id == userId);
            var interes = user.Intereses.FirstOrDefault(e => e.Value == value);
                        
            if (interes != null)
            {
                user.Intereses.Remove(interes);
                await context.SaveChangesAsync();
            }
        }
    }
}
