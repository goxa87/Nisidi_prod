using EventBLib.DataContext;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace EventB.Controllers.Api
{
    //[RoutePrefix("api/Banner")]
    public class BannerController : Controller
    {

        private readonly Context _db;

        
        public BannerController(Context db)
        {
            _db = db;
        }

        
        public async Task<IActionResult> GetBannerByName(string name)
        {
            return PartialView($"~/Views/Banner/{name}");
        }

    }
}
