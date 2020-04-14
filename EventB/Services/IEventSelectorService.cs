using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Services
{
    public interface IEventSelectorService
    {
        public Task<List<Event>> GetStartEventListAsync(User user);
        public Task<List<Event>> GetCostomEventsAsync(CostomSelectionArgs args);
    }
}
