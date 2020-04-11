using EventBLib.Models;
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
        /// Строковые представления интересов пользователя.
        /// </summary>
        public DbSet<Interes> Intereses { get; set; }
        /// <summary>
        /// Список друзей для пользователей.
        /// </summary>
        public DbSet<Friend> Friends { get; set; }

        #region FluentApi Commented
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    //создание таблиц
        //    builder.Entity<Chat>().ToTable("Chat");
        //    builder.Entity<Event>().ToTable("Event");
        //    builder.Entity<Message>().ToTable("Message");
        //    builder.Entity<Person>().ToTable("Person");
        //    builder.Entity<UserChat>().ToTable("UserChat");
        //    builder.Entity<Vizitors>().ToTable("Vizitors");

        //    builder.Entity<UserChat>().HasOne(e => e.Person).WithMany(e => e.UserChat)
        //        .HasForeignKey(e => e.PersonId).OnDelete(DeleteBehavior.Cascade);
        //    builder.Entity<UserChat>().HasOne(e => e.Chat).WithMany(e => e.UserChat)
        //        .HasForeignKey(e => e.ChatId).OnDelete(DeleteBehavior.Cascade);
        //    builder.Entity<Vizitors>().HasOne(e => e.Person).WithMany(e => e.Vizitors)
        //        .HasForeignKey(e => e.PersonId).OnDelete(DeleteBehavior.ClientSetNull);
        //    builder.Entity<Vizitors>().HasOne(e => e.Event).WithMany(e => e.Vizitors)
        //        .HasForeignKey(e => e.EventId).OnDelete(DeleteBehavior.ClientSetNull);
        //}
        #endregion
        
        /// <summary>
        /// Конфигурация поставщика данных.
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(
                @"Server=georgiy-пк\sqlexpress;DataBase=EventBuilder3;Trusted_Connection=True;"
                );
        }

    }
}
