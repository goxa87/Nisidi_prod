using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.FriendsVM
{
    public class FriendsSearchVM
    {
        public List<SmallFigure> Friends { get; set; }
        public FriendSearchParam SearchParam { get; set; }
    }
}
