using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Models.Enums
{
    /// <summary>
    /// Статусы тикетов для фильтрации
    /// </summary>
    public enum TicketFilterStatus
    {
        /// <summary>
        /// Все
        /// </summary>
        all=0,

        /// <summary>
        /// Новые(не взятые в работу)
        /// </summary>
        theNew,

        /// <summary>
        /// Не требуют закрытия
        /// </summary>
        notNeedToClose,

        /// <summary>
        /// Текущего пользователя
        /// </summary>
        currentUsers,

        /// <summary>
        /// Текущего пользователя в зятые в работу
        /// </summary>
        currentUsersInWork
    }
}
