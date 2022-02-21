using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBLib.Enums
{
    /// <summary>
    /// Статусы проверки событий администрацией.
    /// </summary>
    public enum EventCheckStatus
    {
        /// <summary>
        /// Новое выложенное событие (не проверено) требуется ПРОВЕРКА
        /// </summary>
        New = 0,

        /// <summary>
        /// Проверено одобрено (показываем)
        /// </summary>
        Confirmed,

        /// <summary>
        /// Заблокированно (не показываем)
        /// </summary>
        Banned,

        /// <summary>
        /// Изменено по инициативе пользователя (событие не было ранее заблокировано. Показываем) требуется ПРОВЕРКА
        /// </summary>
        Changed,

        /// <summary>
        /// Изменено после блокировки (событие ранее было заблокировано. Не показываем до проверки) требуется ПРОВЕРКА
        /// </summary>
        ChangedAfterBan
    }
}
