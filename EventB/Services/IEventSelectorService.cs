using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services
{
    public interface IEventSelectorService
    {
        public Task<IEnumerable<Event>> GetStartEventListAsync(User user);
        public Task<IEnumerable<Event>> GetCostomEventsAsync();
    }
}
