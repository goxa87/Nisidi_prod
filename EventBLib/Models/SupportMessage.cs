using System;
using System.Collections.Generic;
using System.Text;

namespace EventBLib.Models
{
    /// <summary>
    /// Сообщение в техподдержку
    /// </summary>
    public class SupportMessage
    {
        public int SupportMessageId { get; set; }
        public string Text { get; set; }        
        public DateTime MessageDate { get; set; }        

        public string UserId { get; set; }
        public User Client { get; set; }
        public string ClientName { get; set; }
        public bool IsReadClient { get; set; }

        public string SupportPersonId { get; set; }
        public bool IsReadSupport { get; set; }

        public int SupportChatId { get; set; }
    }
}
