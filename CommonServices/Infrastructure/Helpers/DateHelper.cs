using System;
using System.Collections.Generic;
using System.Text;

namespace CommonServices.Infrastructure.Helpers
{
    public static class DateHelper
    {
        /// <summary>
        /// Привести значение к формату (DDseparatorMMseparatorYYYY)
        /// </summary>
        /// <param name="date"></param>
        /// <param name="numericmonth">Использовать числовой месяц(1) строковый(0)</param>
        /// <param name="separator">То чем будем разделять</param>
        /// <param name="addYear">Добавлять ли год</param>
        /// <returns></returns>
        public static string ToDateStringmonth(this DateTime date, bool numericmonth = true, string separator = ".", bool addYear = true)
        {
            var months = new[]
            {
                "января",
                "февраля",
                "марта",
                "апреля",
                "мая",
                "июня",
                "июля",
                "августа",
                "сентября",
                "октября",
                "ноября",
                "декабря",
            };
            var Day = date.Day.ToString();
            var Month = numericmonth ? date.Month.ToString() : months[date.Month - 1];
            var Year = addYear ? $"{separator}{date.Year.ToString()}" : string.Empty;
            var result = $"{Day}{separator}{Month}{Year}";
            return result;
        }
    }
}
