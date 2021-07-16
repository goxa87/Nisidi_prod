using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBLib.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EventBLib.Models.MarketingModels;

namespace EventBLib.Models
{
    /// <summary>
    /// Возможность просматривать аккаунт другими пользователями.
    /// </summary>
    public enum AccountVisible { Visible, FriendsOnly, Unvisible }

    public class User : IdentityUser
    {
        #region rops
        /// <summary>
        /// Псевдоним пользователя в системе.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Нормализованное имя (Верхний регистр).
        /// </summary>
        public string NormalizedName { get; set; }
        /// <summary>
        /// Город привязки пользователя. 
        /// </summary>
        [MaxLength(50)]
        public string City { get; set; }

        /// <summary>
        /// Нормализованный город.
        /// </summary>
        [MaxLength(50)]
        public string NormalizedCity { get; set; }

        /// <summary>
        /// Адрес фотографии.
        /// </summary>
        [MaxLength(124)]
        public string Photo { get; set; }

        /// <summary>
        /// Сигнализирует о возможности отправлять этому пользователю 
        /// сообщения от пользователей, которые для него не добавлены в друзья.
        /// </summary>
        public bool AnonMessages { get; set; }

        /// <summary>
        /// Видимость данных аккауната для других пользователей.
        /// </summary>
        public AccountVisible Visibility { get; set; }

        /// <summary>
        /// Несколько слов о пользователе.
        /// </summary>
        [MaxLength(1000, ErrorMessage ="Длинна этого поля не более 1000 символов")]
        public string Description { get; set; }

        /// <summary>
        /// Блокировка пользователя.
        /// </summary>
        public bool IsBlockedUser { get; set; }

        /// <summary>
        /// Изображение в среднем формате
        /// </summary>
        [MaxLength(124)]
        public string MediumImage { get; set; }

        /// <summary>
        /// Изображение в маленьком формате
        /// </summary>
        [MaxLength(124)]
        public string MiniImage { get; set; }

        #endregion

        #region Navigation

        /// <summary>
        /// Мои события.
        /// </summary>
        public List<Event> MyEvents { get; set; }

        /// <summary>
        /// Мои отметки Пойду.
        /// </summary>
        public List<Vizit> Vizits { get; set; }

        /// <summary>
        /// Мои интересы.
        /// </summary>
        public List<Interes> Intereses { get; set; }

        /// <summary>
        /// Мои друзья.
        /// </summary>
        public List<Friend> Friends { get; set; }

        /// <summary>
        /// Чаты, в которых участвует пользователь.
        /// </summary>
        public List<UserChat> UserChats { get; set; }

        /// <summary>
        /// Приглашения.
        /// </summary>
        public List<Invite> Invites { get; set; }

        /// <summary>
        /// ЛК пользователя.
        /// </summary>
        public MarketKibnet MarketKibnet { get; set; }

        /// <summary>
        /// Системные баннеры для показа пользователю
        /// </summary>
        public List<UserBanner> UserBanners { get; set; }

        #endregion
    }
}
