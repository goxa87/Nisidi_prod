using EventBLib.Models.MarketingModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBLib.MarketingDataContext
{
    public class MarketingContext : DbContext
    {
        /// <summary>
        /// Список катрочек рекламы для загрузки в списки.
        /// </summary>
        public DbSet<MarkListCard> MarkListCards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Server=georgiy-пк\sqlexpress;DataBase=EB1Marketing;Trusted_Connection=True;");
        }

    }
}
