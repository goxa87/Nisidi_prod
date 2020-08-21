﻿using System;
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
        readonly IUserFindService findUser;
        private readonly IMessageService messagesService;
        public MessagesController(Context _context,
            UserManager<User> _userManager,
            IUserFindService _findUser,
            IMessageService _messagesService)
        {
            context = _context;
            userManager = _userManager;
            findUser = _findUser;
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
                userChats=user.UserChats,
                CurrentUserId = user.Id,
                CurrentUserName = user.Name,
                Opponent = opponent,
                CurrentChatId = 0
            };

            if (!string.IsNullOrWhiteSpace(opponentId))
            {
                var messages = await context.Messages.
                    Where( e => (e.PersonId==user.Id && e.ReciverId==opponentId) || (e.PersonId == opponentId && e.ReciverId == user.Id) )
                    .OrderByDescending(e=>e.PostDate)
                    .Take(30)
                    .OrderBy(e=>e.PostDate)
                    .ToListAsync();
                chatVM.Messages = messages;
                
                if (messages.Any())
                {
                    chatVM.CurrentChatId = messages.First().ChatId;
                }
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
            var chat = await context.UserChats.FirstOrDefaultAsync(e => e.ChatId == chatId);
            var result = await messagesService.DeleteUserChat(chat.UserChatId);
            return StatusCode(result);
        }
    }
}