using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.ViewModels.Account
{
    public class LoginVM
    {
        [Required]
        [MinLength(4)]
        [EmailAddress]
        public string Login { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
