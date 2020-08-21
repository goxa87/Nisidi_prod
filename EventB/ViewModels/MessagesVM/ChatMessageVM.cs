using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.MessagesVM
{
    public class ChatMessageVM
    {
        public int chatId { get; set; }
        public string personId { get; set; }
        public string senderName { get; set; }
        public string text { get; set; }
        public DateTime postDate { get; set; }
    }
}
