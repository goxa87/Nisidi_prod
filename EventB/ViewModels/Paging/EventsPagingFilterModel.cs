using EventBLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventB.ViewModels.Paging
{
    public class EventsPagingFilterModel<T>
    {
        public List<T> Events { get; set; }

        public PagingBaseModel Paging { get; set; }
    }
}
