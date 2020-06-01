using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBLib.MarketingDataContext;
using Microsoft.AspNetCore.Mvc;

namespace EventB.Controllers
{
    public class HelpController : Controller
    {
        readonly MarketingContext marketingContext;

        public HelpController(MarketingContext _marketingContext)
        {
            marketingContext = _marketingContext;
        }

        public IActionResult Index()
        {
            var rez = marketingContext.MarkListCards.First();

            return View(rez);
        }
    }
}