using EventBLib.Models;
using EventBLib.Models.MarketingModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic; 
using System.Linq;
using System.Threading.Tasks;

 
namespace EventBLib.DataContext
{
    public class Context : IdentityDbContext<User>
    {
        public Context(DbContextOptions<Context> options ): base(options)
        {

        }

        /// <summary>
        /// События.
        /// </summary>
        public DbSet<Event> Events { get; set; }

        /// <summary>
        /// Связка пользователей и событий.
        /// </summary>
        public DbSet<Vizit> Vizits { get; set; }

        /// <summary>
        /// Связка чатов с пользователями.
        /// </summary>
        public DbSet<UserChat> UserChats { get; set; }

        /// <summary>
        /// Чаты.
        /// </summary>
        public DbSet<Chat> Chats { get; set; }
        
        /// <summary>
        /// Сообщение.
        /// </summary>
        public DbSet<Message> Messages { get; set; }

        /// <summary>
        /// Сообщения в техподдержку
        /// </summary>
        public DbSet<SupportMessage> SupportMessages { get; set; }

        /// <summary>
        /// Чаты поддержки
        /// </summary>
        public DbSet<SupportChat> SupportChats { get; set; }

        /// <summary>
        /// Строковые представления интересов пользователя.
        /// </summary>
        public DbSet<Interes> Intereses { get; set; }

        /// <summary>
        /// Список друзей для пользователей.
        /// </summary>
        public DbSet<Friend> Friends { get; set; }

        /// <summary>
        /// Список тегов событий.
        /// </summary>
        public DbSet<EventTeg> EventTegs { get; set; }

        /// <summary>
        /// Приглашения на события.
        /// </summary>
        public DbSet<Invite> Invites { get; set; }

        /// <summary>
        /// Личный кабиент пользователя.
        /// </summary>
        public DbSet<MarketKibnet> MarketKibnets { get; set; }

        /// <summary>
        /// Рекламные карточки (пока не добавляем в проект . нет бизнес схемы.)
        /// </summary>
        public DbSet<MarkListCard> MarkListCards { get; set; }

        /// <summary>
        /// Баннеры пользователей
        /// </summary>
        public DbSet<UserBanner> UserBanners { get; set; }

        /// <summary>
        /// Задачи техподдержки
        /// </summary>
        public DbSet<SupportTicket> SupportTickets { get; set; }
    }
}
