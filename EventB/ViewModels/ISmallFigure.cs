using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels
{
    public interface ISmallFigure
    {
        string Image { get; set; }
        string Title { get; set; }
        string Link { get; set; }
    }
}
