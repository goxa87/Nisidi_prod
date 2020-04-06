using EventBLib.DataContext;
using EventBLib.Models;
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

        public User GetCurrentUser(string name)
        {
            var user = context.Users.FirstOrDefault(e => e.UserName == name);
            return user;
        }

        public User GetUserById(string id)
        {
            var user = context.Users.FirstOrDefault(e => e.Id==id);
            return user;
        }

        public User GetUserByName(string name)
        {
            var user = context.Users.FirstOrDefault(e => e.Name == name);
            return user;
        }
    }
}
