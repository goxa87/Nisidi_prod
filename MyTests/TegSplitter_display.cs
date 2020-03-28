using System;
using System.Collections.Generic;
using System.Text;

namespace MyTests
{
    public class TegSplitter_display
    {
        string originText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">пример тегов которые нужно разбить</param>
        public TegSplitter_display(string text)
        {
            originText = text;
        }

        public string GetSplitTegs()
        {
            ////var arr = EventLib.StringSrevices.TegSplitter.GetEnumerable(originText);
            //StringBuilder SB = new StringBuilder();
            //foreach (var e in arr)
            //{
            //    SB.Append(e);
            //    SB.Append('\n');
            //}


            return "SB.ToString()";
        }
    }
}
