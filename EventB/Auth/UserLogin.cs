using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.Auth
{
    public class UserLogin
    {
        [Required]
        public string LoginProp { get; set; }

        [DataType(DataType.Password),Required]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
