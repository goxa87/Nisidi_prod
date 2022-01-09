using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBLib.Enums
{
    public enum SupportTicketStatus
    {
        /// <summary>
        /// Новая
        /// </summary>
        New = 0,

        /// <summary>
        /// Сейчас в работе
        /// </summary>
        InWork,

        /// <summary>
        /// Закрыта
        /// </summary>
        Closed, 

        /// <summary>
        /// Удалена
        /// </summary>
        Deleted
    }
}
