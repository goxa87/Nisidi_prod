using Admin.AdminDbContext.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.AdminDbContext
{
    public class AdminContext : IdentityDbContext<AdminUser>
    {

        public AdminContext(DbContextOptions<AdminContext> options) : base(options)
        {

        }
    }
}
