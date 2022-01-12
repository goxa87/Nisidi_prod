using CommonServices.Infrastructure.WebApi;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace EventB.Controllers.Api
{
    public class BannerController : Controller
    {

        private readonly Context _db;

        
        public BannerController(Context db)
        {
            _db = db;
        }

        /// <summary>
        /// Вернет разметку баннера с указанным именем файла
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetBannerByName(string name)
        {
            var username = User?.Identity?.Name;
            ViewData["show"] = "true";
            if(username != null)
            {
                var user = await _db.Users.FirstOrDefaultAsync(e => e.UserName == username);
                var banner = await _db.UserBanners.FirstOrDefaultAsync(e => e.UserId == user.Id && e.BannerName == name);
                if(banner != null && banner.ToShow == false)
                {
                    ViewData["show"] = "false";
                }
            }
            return PartialView($"~/Views/Banner/{name}");
        }

        /// <summary>
        /// Пользователь закроет баннер
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<WebResponce<string>> CloseBanner(string bannerName)
        {
            if(User?.Identity?.Name != null)
            {
                var user = await _db.Users.FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);
                var userBanner = new UserBanner()
                {
                    UserId = user.Id,
                    BannerName = bannerName,
                    ToShow = false
                };
                _db.Update(userBanner);
                await _db.SaveChangesAsync();
                return new WebResponce<string>("Запись создана");
            }
            return new WebResponce<string>("Пользователь не идентифицирован", false, "Пользователь не идентифицирован");
        }
    }
}
