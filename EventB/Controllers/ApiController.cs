using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBLib.DataContext;
using EventBLib.Models;
using EventB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventB.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        readonly Context context;
        readonly IUserFindService userFind;
        public ApiController(
            Context _context,
            IUserFindService _userFind
            )
        {
            context = _context;
            userFind = _userFind;
        }
        [Route("AddFriend")]
        [Authorize]
        public async Task<StatusCodeResult> AddAsFriend(string userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var currentUser = userFind.GetCurrentUser(User.Identity.Name);
                var friend = userFind.GetUserById(userId);

                var userFriend = new Friend
                {
                    UserId = currentUser.Id,
                    PersonFriendId = friend.Id,
                    UserName = friend.Name,
                    UserPhoto = friend.Photo
                };

                // await context.Friends.AddAsync(userFriend);

                return StatusCode(200);
            }
            else
            {
                return StatusCode(404);
            }           
        }
    }
}