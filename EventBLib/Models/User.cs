using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventB.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventBLib.Models
{
    public class User : IdentityUser
    {
        /// <summary>
        /// Псевдоним пользователя в системе.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Город привязки пользователя. 
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Photo { get; set; }

        public List<Event> MyEvents { get; set; }

        public List<Vizit> Vizits { get; set; }

        public List<Interes> Intereses { get; set; }

        public List<Friend> Friends { get; set; }

        public List<UserChat> UserChats { get; set; }

        
    }
}
