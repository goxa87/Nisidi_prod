using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EventB.Services;
using EventB.Services.ImageService;
using EventB.ViewModels.MyPage;
using EventBLib.DataContext;
using EventBLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventB.Controllers
{
    /// <summary>
    /// Моя страница.
    /// </summary>
    [Authorize]
    public class MyPageController : Controller
    {
        private const string IMAGE_SUFFIX = ".jpeg";

        readonly Context context;
        readonly UserManager<User> userManager;
        readonly ITegSplitter tegSplitter;
        readonly IWebHostEnvironment environment;
        private readonly IImageService imageService;


        public MyPageController(Context _context,
            UserManager<User> _userManager,
            ITegSplitter _tegSplitter,
            IWebHostEnvironment _environment,
            IImageService _imageService)
        {
            context = _context;
            userManager = _userManager;
            tegSplitter = _tegSplitter;
            environment = _environment;
            imageService = _imageService;
        }
        /// <summary>
        /// Страница профиля.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var user = await context.Users.
                Include(e => e.Intereses).
                Include(e => e.MyEvents).
                Include(e => e.Vizits).
                ThenInclude(e => e.Event).
                Include(e => e.Invites).
                ThenInclude(e => e.Event).
                FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);

            var friends = await context.Friends.Where(e => e.FriendUserId == user.Id && e.IsBlocked == false).ToListAsync();
            user.Friends = friends;
            return View(user);
        }

        /// <summary>
        /// Подтверждение приглашения.
        /// </summary>
        /// <param name="eventId">Ид события.</param>
        /// <param name="inviteId">Ид приглашения.</param>
        /// <returns></returns>
        [Route("/MyPage/SubmitInvite")]
        public async Task<StatusCodeResult> SubmitInvite(int eventId, int inviteId)
        {
            var curUser = await userManager.FindByNameAsync(User.Identity.Name);
            var eve = await context.Events.Include(e => e.Chat).FirstOrDefaultAsync(e => e.EventId == eventId);
            var inv = await context.Invites.FirstOrDefaultAsync(e => e.InviteId == inviteId);
            if (curUser.Id != inv.UserId)
            {
                return StatusCode(401);
            }
            if (curUser == null || eve == null || inv == null)
            {
                return StatusCode(410);
            }
            // Если визит уже есть
            if (await context.Vizits.AnyAsync(e => e.EventId == eventId && e.UserId == curUser.Id))
            {
                return Ok();
            }
            // Добавляем Визит.
            var newVizit = new Vizit
            {
                UserId = curUser.Id,
                EventId = eve.EventId,
                EventTitle = eve.TitleShort,
                EventPhoto = eve.MediumImage,
                VizitorName = curUser.Name,
                VizitirPhoto = curUser.MediumImage
            };
            context.Vizits.Add(newVizit);
            var userChat = new UserChat
            {
                ChatId = eve.Chat.ChatId,
                ChatName = eve.TitleShort,
                ChatPhoto = eve.MiniImage,
                UserId = curUser.Id,
                SystemUserName = curUser.UserName
            };
            context.UserChats.Add(userChat);
            // Удаляем приглашение.
            context.Invites.Remove(inv);
            await context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Отказ от приглашения.
        /// </summary>
        /// <param name="inviteId">Ид приглашения.</param>
        /// <returns></returns>
        [Route("/MyPage/RefuseInvite")]
        public async Task RefuseInvite(int inviteId)
        {
            var inv = await context.Invites.FirstOrDefaultAsync(e => e.InviteId == inviteId);
            if (inv == null)
            {
                Response.StatusCode = 410;
                return;
            }
            context.Invites.Remove(inv);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Заполнение формы для редактирования профиля.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("MyPage/EditProfile")]
        public async Task<IActionResult> EditProfile()
        {
            var curUser = await userManager.GetUserAsync(HttpContext.User);
            var tegs = await context.Intereses.Where(e => e.UserId == curUser.Id).ToListAsync();
            var tegsStr = "";
            foreach (var teg in tegs)
            {
                tegsStr = $"{tegsStr} {teg.Value}";
            }
            var VM = new EditProfileVM
            {
                Name = curUser.Name,
                OldName = curUser.Name,
                OldPhoto = curUser.Photo,
                City = curUser.City,
                Tegs = tegsStr,
                OldTegs = tegsStr,
                Description = curUser.Description,
                AlowAnonMessages = curUser.AnonMessages,
                Visibility = curUser.Visibility,
                PhoneNumber = curUser.PhoneNumber
            };

            return View(VM);
        }
        /// <summary>
        /// Редактирование профиля.
        /// </summary>
        /// <param name="model">VM от пользователя.</param>
        /// <returns>Редирект к моей странице.</returns>
        [HttpPost]
        [Route("MyPage/EditProfile")]
        public async Task<IActionResult> EditProfile(EditProfileVM model)
        {
            if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.City))
            {
                ModelState.AddModelError("", "Заполните поля Имя и Город");
                return View(model);
            }
            else
            {
                // Сдесь всю шляпу получать за раз что нужно менять
                var user = await context.Users.Include(e => e.Friends)
                    .Include(e => e.Invites)
                    .Include(e => e.Vizits)
                    .FirstOrDefaultAsync(e => e.UserName == HttpContext.User.Identity.Name);
                List<Friend> inFriends = user.Friends;
                List<Invite> inInvites = await context.Invites.Where(e => e.InviterId == user.Id).ToListAsync();
                List<Vizit> inVizits = user.Vizits;
                // Это юзер чаты в которых участвует, но привязвны к другим пользователям (Приватые). 
                List<UserChat> inUserChats = await context.UserChats.Where(e => e.OpponentId == user.Id).ToListAsync();

                if (model.newPhoto != null)
                {
                    var userImgDict = new Dictionary<int, string>();
                    userImgDict.Add(400, TrimSuffix(user.Photo));
                    userImgDict.Add(360, TrimSuffix(user.MediumImage));
                    userImgDict.Add(100, TrimSuffix(user.MiniImage));

                    await imageService.SaveOriginAndResizedImagesByInputedSizes(model.newPhoto, IMAGE_SUFFIX, userImgDict, null);
                } 

                // Изменения имени в таблицах.
                if (model.Name != model.OldName)
                {
                    user.Name = model.Name;
                    user.NormalizedName = model.Name.ToUpper();
                    foreach (var e in inFriends)
                    {
                        e.UserName = model.Name;
                    }
                    foreach (var e in inInvites)
                    {
                        e.InviterName = model.Name;
                    }
                    foreach (var e in inVizits)
                    {
                        e.VizitorName = model.Name;
                    }                 
                    foreach(var e in inUserChats)
                    {
                        e.ChatName = model.Name;
                    }
                    foreach(var e in await context.Messages.Where(e=>e.PersonId == user.Id).ToListAsync())
                    {
                        e.SenderName = model.Name;
                    }
                }
                // Изменение интересов.
                if (model.Tegs != model.OldTegs)
                {
                    var oldTegsDb = context.Intereses.Where(e => e.UserId == user.Id);
                    context.Intereses.RemoveRange(oldTegsDb);
                    var newTegs = tegSplitter.GetEnumerable(model.Tegs.ToUpper())
                        .Select(e => new Interes { Value = e })
                        .ToList();
                    user.Intereses = newTegs;
                }
                // Прочие изменения.
                user.City = model.City;
                user.NormalizedCity = model.City.ToUpper();
                user.Description = model.Description;
                user.AnonMessages = model.AlowAnonMessages;
                user.Visibility = model.Visibility;
                user.PhoneNumber = model.PhoneNumber;
                context.Users.Update(user);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }

        #region private
        /// <summary>
        /// Вернет строчку без расширения файла
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string TrimSuffix(string path)
        {
            return path.Substring(0, path.LastIndexOf('.'));
        }
        #endregion
    }
}