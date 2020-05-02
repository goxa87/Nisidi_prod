using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.EventsVM
{
    public class InviteInVm
    {
        // Id Пользователя.
        public string userId { get; set; }
        // Сообщение пользователю при приглашении.
        public string message { get; set; }
    }
}
