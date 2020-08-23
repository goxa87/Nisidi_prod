using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.Logger
{
    public interface ILogger
    {
        Task LogStringToFile(string message);

        Task LogObjectToFile(string notaition, object data);

        Task LogToDB(object data);
    }
}
