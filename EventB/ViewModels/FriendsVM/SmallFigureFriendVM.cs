using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.FriendsVM
{
    public class SmallFigureFriendVM : ISmallFigure
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
    }
}
