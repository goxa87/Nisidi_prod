using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services
{
    /// <summary>
    /// Разбивает строку тегов на список
    /// </summary>
    public class TegSplitter:ITegSplitter
    {
        public IEnumerable<string> GetEnumerable(string input)
        {
            if (input != "")
                return input.Split(' ', ',', '.', '/', ':', '*', '+', '-', '@').Where(e => !string.IsNullOrEmpty(e));
            else 
                return null;
        }
    }
}
