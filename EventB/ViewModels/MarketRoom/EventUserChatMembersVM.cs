using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.MarketRoom
{
    public class EventUserChatMembersVM
    {
        public string UserId { get; set; }
        public string UserPhoto { get; set; }
        public string UserName { get; set; }
        public int UserChatId { get; set; }
        public bool IsBlocked { get; set; }
    }
}
