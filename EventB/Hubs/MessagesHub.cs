using EventB.Hubs.Models;
using EventB.Services.MessageServices;
using EventB.ViewModels.MessagesVM;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EventB.Hubs
{
    public class MessagesHub : Hub
    {
        private readonly Context context;
        private readonly UserManager<User> userManager;

        private readonly IMessageService messageService;

        public MessagesHub(Context _context,
            UserManager<User> _userManager,
            IMessageService _messageService)
        {
            context = _context;
            userManager = _userManager;
            messageService = _messageService;
        }

        /// <summary>
        /// При подключении нового соединения.
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {            
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            await base.OnConnectedAsync();           
        }
        /// <summary>
        /// Отключение (удаление контейнера внутри)
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// отправляет сообщение в чат.
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public async Task SendToChat(ChatMessageVM dataObject)
        {
            var messageDTO = messageService.ConvertMessageVmToDTO(dataObject);
            context.Messages.Add(messageDTO);
            await context.SaveChangesAsync();

            var chat = await context.Chats.Include(e => e.UserChat).ThenInclude(e=>e.User).FirstOrDefaultAsync(e => e.ChatId == dataObject.chatId);
            var usersForSending = chat.UserChat.Select(e => e.User.UserName);

            foreach(var userName in usersForSending)
            {
                if(userName!=Context.User.Identity.Name)
                    await this.Clients.Group(userName).SendAsync("reciveChatMessage", dataObject);
            }

        }
    }
}
