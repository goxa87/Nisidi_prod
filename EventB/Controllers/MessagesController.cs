using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventB.Services;
using Microsoft.EntityFrameworkCore;
using EventBLib.DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventBLib.Models;
using EventB.ViewModels;
using Microsoft.AspNetCore.Identity;
using EventB.Services.MessageServices;

namespace EventB.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        readonly Context context;
        private readonly UserManager<User> userManager;
        private readonly IMessageService messagesService;
        public MessagesController(Context _context,
            UserManager<User> _userManager,
            IUserFindService _findUser,
            IMessageService _messagesService)
        {
            context = _context;
            userManager = _userManager;
            messagesService = _messagesService;
        }

        public async Task<IActionResult> Index(string opponentId=null)
        {
            // Соединения текщего пользователя.
            var user = await context.Users.
                Include(e => e.UserChats).
                FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);
            var opponent = await context.Users.FirstOrDefaultAsync(e => e.Id == opponentId);
            var chatVM = new MessagesViewModel
            {
                UserChats = user.UserChats.Where(e => e.IsDeleted == false && e.IsBlockedInChat == false).OrderBy(e => e.ChatName).ToList(),
                CurrentUserId = user.Id,
                CurrentUserName = user.Name,
                Opponent = opponent,
                CurrentChatId = 0
            };

            if (!string.IsNullOrWhiteSpace(opponentId))
            {
                var userChat = await context.UserChats.Include(e => e.Chat).ThenInclude(e => e.Messages).FirstOrDefaultAsync(e => e.UserId == user.Id && e.OpponentId == opponentId);
                chatVM.CurrentChatId = userChat != null ? userChat.ChatId : 0;
                chatVM.Messages = userChat != null ? userChat.Chat.Messages : new List<Message>();
            }

            return View(chatVM);
        }

        /// <summary>
        /// Удаляет useerChat пользователя.
        /// </summary>
        /// <param name="сhatId">id чата</param>
        /// <returns></returns>
        [Route("/messages/delete-user-chat")]
        public async Task<StatusCodeResult> DeleteUserChat(int chatId)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var result = await messagesService.DeleteUserChat(chatId, user.Id);
            return StatusCode(result);
        }
    }
}