using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventB.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventB.Auth
{
    public class User : IdentityUser
    {
        /// <summary>
        /// сыылка на класс пользователя
        /// </summary>
        public int PersonId { get; set; }
        /// <summary>
        /// аккаунт пользователя
        /// </summary>
        public Person Person { get; set; }
    }
}
