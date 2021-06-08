using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.AdminDbContext.Models
{
    public class AdminUser: IdentityUser
    {
        public string UserFio { get; set; }
    }
}
