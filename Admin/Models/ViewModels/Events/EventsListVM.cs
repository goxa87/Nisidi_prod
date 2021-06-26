using CommonServices.Infrastructure.Paging;
using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.ViewModels.Events
{
    public class EventsListVM
    {
        public PagingParam PagingParam { get; set; }
        public List<Event> Events { get; set; }
    }
}
