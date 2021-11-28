using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.FriendsVM
{
    public class FriendsListVM
    {
        public List<Friend> Friends { get; set; }
        public FriendSearchParam SearchParam { get; set; }
    }
}
