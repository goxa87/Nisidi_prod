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

namespace EventB.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        readonly Context context;
        readonly IUserFindService findUser;
        public MessagesController(Context _context,
            IUserFindService _findUser)
        {
            context = _context;
            findUser = _findUser;
        }

        public async Task<IActionResult> Index(string opponentId="")
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
                #region гемор с поиском сообщений через UserChat
                //// Оппонент
                //var opponent = await context.Users.
                //Include(e => e.UserChats).
                //ThenInclude(e => e.Chat).
                //FirstOrDefaultAsync(e => e.Id == opponentId);
                //// Ищем общие чаты. Могут быть частные а могут быть и рупповые к событиям.
                //// Выбираем те которые частные для текущего пользователя и оппонента.

                //// Массив чатов, где участвует текущий.
                //var chatCurent = context.Chats.
                //    Include(e => e.UserChat).
                //    ThenInclude(e => e.User).
                //    Where(e => e.UserChat.Any(u => u.User == user));
                //// Массив чатов где участыует оппонент.
                //var chatOpponent = context.Chats.
                //    Include(e => e.UserChat).
                //    ThenInclude(e => e.User).
                //    Where(e => e.UserChat.Any(u => u.User == opponent));
                //// Пересечение этих массивов.
                //var commonChats = chatCurent.Intersect(chatOpponent);

                //if (!commonChats.Any())
                //{
                //    // Создать чат

                //}
                //else
                //{ 
                //    // Взять чат из результата.
                //}
                ////var commonChats = user.
                #endregion
                // Будет несоответствие если подзагрузку делать иначе.
                // Переделать к общему виду.
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
    }
}