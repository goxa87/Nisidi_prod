using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services
{
    public class UserFindService : IUserFindService
    {
        readonly Context context;

        public UserFindService(Context _context)
        {
            context = _context;
        }

        public async Task<User> GetCurrentUserAsync(string name)
        {
            var user = await context.Users.FirstOrDefaultAsync(e => e.UserName == name);
            return user;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            var user = await context.Users.FirstOrDefaultAsync(e => e.Id==id);
            return user;
        }

        public async Task<User> GetUserByNameAsync(string name)
        {
            var user = await context.Users.FirstOrDefaultAsync(e => e.Name == name);
            return user;
        }
    }
}
