using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services
{
    public interface IUserFindService
    {
        public User GetUserById(string id);
        public User GetUserByName(string name);
        public User GetCurrentUser(string name);
    }
}
