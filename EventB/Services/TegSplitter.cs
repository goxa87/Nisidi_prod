using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services
{
    /// <summary>
    /// Разбивает строку тегов на список
    /// </summary>
    public class TegSplitter : ITegSplitter
    {
        public List<string> GetEnumerable(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
                return input.ToUpper().Split(new char[]{ ' ', ',', '.', '/', ':', '*', '+', '-', '@','?','!','='}, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            else 
                return null;
        }
    }
}
