using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services
{
    /// <summary>
    /// Разбивает строку тегов на список
    /// </summary>
    public class TegSplitter
    {
        public static IEnumerable<string> GetEnumerable(string input)
        {
            return input.Split(' ', ',', '.', '/', ':', '*', '+', '-', '@').Where(e => !string.IsNullOrEmpty(e));
        }
    }
}
