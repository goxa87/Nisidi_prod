using Admin.Models.ViewModels.CommonUsers;
using CommonServices.Infrastructure.WebApi;
using EventBLib.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Services.CommonUsersService
{
    public class CommonUsersService : ICommonUsersService
    {
        /// <summary>
        /// Контекст БД нисиди
        /// </summary>
        private readonly Context _db;

        /// <summary>
        /// Конфигурация приложения.
        /// </summary>
        private readonly IConfiguration _configuration;


        private readonly ILogger<CommonUsersService> _logger;

        public CommonUsersService(Context db,
            IConfiguration configuration,
            ILogger<CommonUsersService> logger)
        {
            _db = db;
            _configuration = configuration;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ListVM> GetUsersListByFilter(ListFilterParam param)
        {
            var query = _db.Users.AsQueryable();
            if (!string.IsNullOrEmpty(param.City))
            {
                query = query.Where(e => EF.Functions.Like(e.NormalizedCity, $"%{param.City.ToUpper()}%"));
            }

            if (!string.IsNullOrEmpty(param.Name))
            {
                query = query.Where(e => EF.Functions.Like(e.NormalizedName, $"%{param.Name.ToUpper()}%"));
            }

            if (!string.IsNullOrEmpty(param.UserId))
            {
                query = query.Where(e => e.Id == param.UserId);
            }

            if (param.RegistrationDateStart.HasValue)
            {
                query = query.Where(e => e.RegistrationDate > param.RegistrationDateStart.Value.Date);
            }

            if (param.RegistrationDateEnd.HasValue)
            {
                query = query.Where(e => e.RegistrationDate < param.RegistrationDateEnd.Value.Date.AddDays(1));
            }

            return new ListVM() {
                FilterParam = param,
                Users = await query.OrderByDescending(e=>e.RegistrationDate).ToListAsync()
            };
        }

        /// <inheritdoc />
        public async Task<UserDetailVM> GetUsersDetails(string userId)
        {

            var user = await _db.Users.Include(e=>e.Intereses).FirstAsync(e => e.Id == userId);
            return new UserDetailVM
            {
                User = user,
                ProfileImageUrl = $"{_configuration.GetValue<string>("RootHostNisidi")}{user.Photo}"
            };
        }


        #region ajax
        /// <inheritdoc />
        public async Task<WebResponce<bool>> ConfirnUserEmail(string userId)
        {
            try
            {
                var user = await _db.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new Exception($"Не найден пользователь с ID {userId}");
                }

                user.EmailConfirmed = true;
                await _db.SaveChangesAsync();
                return new WebResponce<bool>(true);
            }
            catch(Exception ex)
            {
                _logger.LogError($"{ex.Message} - \n{ex.StackTrace}");
                return new WebResponce<bool>(false, false, $"ошибка ConfirnUserEmail > {ex.Message}");
            }
        }

        /// <inheritdoc />
        public async Task<WebResponce<bool>> ToggleBlockUser(string userId)
        {
            try
            {
                var user = await _db.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new Exception($"Не найден пользователь с ID {userId}");
                }

                user.IsBlockedUser = !user.IsBlockedUser;
                await _db.SaveChangesAsync();
                return new WebResponce<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - \n{ex.StackTrace}");
                return new WebResponce<bool>(false, false, $"ошибка ToggleBlockUser > {ex.Message}");
            }
        }

        #endregion
    }
}
