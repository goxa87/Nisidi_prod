﻿using System;
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
            if (input != "")
                return input.ToLower().Split(new char[]{ ' ', ',', '.', '/', ':', '*', '+', '-', '@','?','!','='}, StringSplitOptions.RemoveEmptyEntries).ToList();
            else 
                return null;
        }
    }
}
