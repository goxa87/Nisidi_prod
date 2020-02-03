using EventB.Auth;
using EventB.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

 
namespace EventB.DataContext
{
    public class Context : IdentityDbContext<User>
    {
        //public Context(DbContextOptions opt) : base(opt) { }
        /// <summary>
        /// Чаты
        /// </summary>
        public DbSet<Chat> Chats { get; set; }
        /// <summary>
        /// События
        /// </summary>
        public DbSet<Event> Events { get; set; }
        /// <summary>
        /// Сообщение
        /// </summary>
        public DbSet<Message> Messages { get; set; }
        /// <summary>
        /// профиль пользователя
        /// </summary>
        public DbSet<Person> Persons { get; set; }
        /// <summary>
        /// связка чатов с пользователями
        /// </summary>
        public DbSet<UserChat> UserChats { get; set; }
        /// <summary>
        /// связка пользователей и событий
        /// </summary>
        public DbSet<Vizitors> Vizitors { get; set; }

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

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(
                @"Server=georgiy-пк\sqlexpress;DataBase=EventBuilder1;Trusted_Connection=True;"
                );
        }

    }
}
