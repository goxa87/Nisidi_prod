using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventB.ViewModels.MessagesVM
{
    public class NewMessagesVM
    {
        /// <summary>
        /// ИД чата.
        /// </summary>
        public int ChatId { get; set; }
        /// <summary>
        /// количество новых.
        /// </summary>
        public int CountNew { get; set; }

    }
}
