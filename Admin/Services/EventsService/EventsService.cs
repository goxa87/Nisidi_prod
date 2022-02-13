using Admin.AdminDbContext;
using Admin.Models.ViewModels.Events;
using Admin.Services.SupportService;
using CommonServices.Infrastructure.WebApi;
using EventBLib.DataContext;
using EventBLib.Enums;
using EventBLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Admin.Services.EventsService
{

    public class EventsService : IEventsService
    {
        /// <summary>
        /// Контекст БД для админки
        /// </summary>
        private readonly AdminContext adminDb;

        /// <summary>
        /// Контекст БД сайта
        /// </summary>
        private readonly Context db;

        /// <summary>
        /// Конфигурация приложения.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Сервис заявкок в поддержку
        /// </summary>
        private readonly ISupportService _supportService;

        /// <summary>
        /// логгирование
        /// </summary>
        private readonly ILogger<EventsService> _logger;

        public EventsService(AdminContext _adminDb,
            Context _context,
            IConfiguration configuration,
            ISupportService supportService,
            ILogger<EventsService> logger)
        {
            adminDb = _adminDb;
            db = _context;
            _configuration = configuration;
            _supportService = supportService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<WebResponce<string>> BanEventByReason(int eventId, string messageToUser, string employeeId)
        {
            var eve = await db.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            if (eve == null)
            {
                var resp = new WebResponce<string>("Событие с указанным id не найдено", false, "Событие с указанным id не найдено");
                resp.ErrorMessage = "Событие с указанным id не найдено";
                return resp;
            }

            eve.CheckStatus = EventCheckStatus.Banned;
            await db.SaveChangesAsync();

            // TODO отправка сообщения в чат от имени нисиди с сообщением о том что нужно изменить
            var newTicket = new SupportTicket()
            {
                Theme = "Блокировка события.",
                Description = $"Событие было заблокировано по причине: {messageToUser}. Оно не будет доступно ля просмотра и не будет отображаться в поиске. Внестие необходимыве изменения в описание события для его разблокировки. ",
                EventId = eventId,
                NeedToClose = false,
                NisidiEmployeeId = _configuration.GetValue<string>("DefaultSupportEmployeeNisidiUserId"),
                OpenDate = DateTime.Now,
                Status = SupportTicketStatus.New,
                SupportEmployeeId = employeeId,
                UserId = eve.UserId
            };
            await _supportService.SaveOrUpdateTicket(newTicket);

            return new WebResponce<string>("Событие успешно ззаблокировано.");
        }

        /// <inheritdoc />
        public async Task<WebResponce<string>> ConfirmEventStatus(int eventId)
        {
            var eve = await db.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            if(eve == null)
            {
                var resp = new WebResponce<string>("Событие с указанным id не найдено", false, "Событие с указанным id не найдено");
                resp.ErrorMessage = "Событие с указанным id не найдено";
                return resp;
            }

            eve.CheckStatus = EventCheckStatus.Confirmed;
            await db.SaveChangesAsync();

            return new WebResponce<string>("Событие успешно подтверждено.");
        }

        ///<inheritdoc />
        public async Task<WebResponce<bool>> DeleteEvent(int eventId, string userId)
        {
            var eve = await db.Events.FirstAsync(e=>e.EventId == eventId);
            _logger.LogInformation($"удаление события:\n{eve.EventId}\nудаляет сотрудник: {userId}\n{eve.Title}\n{eve.Body}\n{eve.City}\n{eve.Place}\n{eve.TicketsDesc}");
            using (var client = new HttpClient())
            {
                var nisidiUrl = _configuration.GetValue<string>("RootHostNisidi");
                var token = _configuration.GetValue<string>("DeleteEventToken");
                client.BaseAddress = new Uri(nisidiUrl);
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"/api/MarketKibnet/delete-event-admin?eventId={eventId}&token={token}"));
                if (response.IsSuccessStatusCode)
                {
                    return new WebResponce<bool>(true);
                }
                else
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Ошибка запроса удаления события: {responseText}.", inner: null, statusCode: response.StatusCode);
                }
            }
        }

        ///<inheritdoc />
        public async Task<Event> GetEventDetails(int eventId)
        {
            var eve = await db.Events.Include(e=>e.EventTegs).Include(e=>e.Creator).Include(e=>e.Vizits).FirstOrDefaultAsync(e=>e.EventId == eventId);
            eve.Image = _configuration.GetValue<string>("RootHostNisidi") + eve.Image;
            return eve;
        }

        ///<inheritdoc />
        public async Task<EventsListVM> GetEventsList(EventListParam param)
        {
            var query = db.Events.Include(e => e.EventTegs).Include(e=>e.Creator).AsQueryable();

            if (param.OnlyRequereCheck)
            {
                var statuses = new List<EventCheckStatus>() { EventCheckStatus.Changed, EventCheckStatus.ChangedAfterBan, EventCheckStatus.New };
                query = query.Where(e => statuses.Contains(e.CheckStatus));
            }

            if (param.EventCreateStartDate.HasValue)
            {
                query = query.Where(e => e.CreationDate > param.EventCreateStartDate);
            }

            if (param.EventCreateEndDate.HasValue)
            {
                query = query.Where(e => e.CreationDate < param.EventCreateEndDate);
            }
            var i = await query.ToListAsync();
            if (param.StartDate.HasValue)
            {
                query = query.Where(e => e.Date >= param.StartDate);
            }
             i = await query.ToListAsync();

            if (param.EndDate.HasValue)
            {
                query = query.Where(e => e.Date <= param.EndDate);
            }
            i = await query.ToListAsync();
            if (!string.IsNullOrEmpty(param.EventTitle))
            {
                query = query.Where(e => EF.Functions.Like(e.Title, $"%{param.EventTitle.Trim()}%"));
            }

            if (!string.IsNullOrEmpty(param.UserName))
            {
                query = query.Where(e => e.Creator.NormalizedUserName == param.UserName.ToUpper() 
                    || e.Creator.NormalizedName == param.UserName.ToUpper());
            }

            if (param.IsGlobal)
            {
                query.Where(e => e.Type == EventBLib.Models.EventType.Global);
            }

            return new EventsListVM() { Events = await query.ToListAsync(), SearchParam = param };
        }

        ///<inheritdoc />
        public async Task<List<Event>> GetEventListByUserId(string userId, bool noTracking)
        {
            if (noTracking)
            {
                return await db.Events.AsNoTracking().Where(e => e.UserId == userId).ToListAsync();
            }
            return await db.Events.Where(e => e.UserId == userId).ToListAsync();
        }
    }
}
