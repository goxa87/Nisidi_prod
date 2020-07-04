using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services.MarketKibnetApiServices
{
    public interface IMarketKibnetApiServices
    {
        Task<bool> ChangeEventStatus(int newType, int eventId, string userName);
        Task<bool> DeleteEvent(int eventId, string userName);
    }
}
