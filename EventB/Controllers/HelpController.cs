using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventB.Controllers
{
    public class HelpController : Controller
    {
        private readonly Context context;
        private readonly UserManager<User> userManager;
        public HelpController(Context _context,
            UserManager<User> _userManager)
        {
            context = _context;
            userManager = _userManager;
        }
        public IActionResult Manual()
        {
            return View();
        }
        public IActionResult FAQ()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> SupportMail()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var messages = await context.SupportMessages.Where(e => e.UserId == user.Id).OrderBy(e => e.MessageDate).ToListAsync();
            ViewBag.UserId = user.Id;
            return View(messages);
        }

        /// <summary>
        /// Отпароить письмо в поддержку.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [Authorize]
        [Route("/help/message-support")]
        public async Task<StatusCodeResult> SendSupportMessage(string text)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var supportChat = await context.SupportChats.Include(e => e.Messages).FirstOrDefaultAsync(e => e.ClientId == user.Id);
            if(supportChat == null)
            {
                supportChat = new SupportChat()
                {
                    ClientId = user.Id,
                    Messages = new List<SupportMessage>()
                };
                context.SupportChats.Add(supportChat);
            }            
            var message = new SupportMessage
            {
                Client = user,
                ClientName = user.Name,
                MessageDate = DateTime.Now,
                Text = text
            };
            supportChat.Messages.Add(message);
            await context.SaveChangesAsync();
            return Ok();
        }
    }

}