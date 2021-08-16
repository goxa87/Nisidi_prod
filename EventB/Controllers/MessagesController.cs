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
using EventB.Services.Logger;
using System.Security.Claims;

namespace EventB.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        readonly Context context;
        private readonly UserManager<User> userManager;
        private readonly IMessageService messagesService;
        private readonly ILogger logger;
        public MessagesController(Context _context,
            UserManager<User> _userManager,
            IUserFindService _findUser,
            IMessageService _messagesService,
            ILogger _logger)
        {
            context = _context;
            userManager = _userManager;
            messagesService = _messagesService;
            logger = _logger;
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
                var userChat = user.UserChats.FirstOrDefault(e => e.OpponentId == opponentId);
                chatVM.CurrentChatId = userChat != null ? userChat.ChatId : 0;
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

        /// <summary>
        /// Отметить чат как прочитанный.
        /// </summary>
        /// <param name="userChatId"></param>
        /// <returns></returns>
        [Route("/messages/mark-as-read-chat")]
        public async Task<StatusCodeResult> MarkAsReadMessage(int chatId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await messagesService.MarkAsReadMessage(chatId, userId);
                return Ok();
            }
            catch(Exception ex)
            {
                await logger.LogStringToFile($"ОШИБКА Отметка как прочитанного сообщения(userChatId: {chatId}): {ex.Message}");
                return Ok();
            }
        }
    }
}