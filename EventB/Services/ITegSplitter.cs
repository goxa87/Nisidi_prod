using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services
{
    public interface ITegSplitter
    {
        public IEnumerable<string> GetEnumerable(string input);
    }
}
