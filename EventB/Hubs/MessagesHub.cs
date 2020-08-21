using EventB.ViewModels.MessagesVM;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Hubs
{
    public class MessagesHub : Hub
    {
        public async Task SendToChat(ChatMessageVM dataObject)
        {


            await this.Clients.All.SendAsync("ReciveMessageFromHub", dataObject);
        }
    }
}
