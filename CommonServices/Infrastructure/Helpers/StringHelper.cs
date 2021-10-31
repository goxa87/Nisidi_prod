using System;
using System.Collections.Generic;
using System.Text;

namespace CommonServices.Infrastructure.Helpers
{
    public static class StringHelper
    {
        public static string Alphabet { get; set; } = "QqWwEeRrTtYyUuIiOoPpAaSsDdFfGgHhJjKkLlZzXxCcVvBbNnMm1234567890!@#$%^&*()=+-_";
        public static string Letters { get; set; } = "QqWwEeRrTtYyUuIiOoPpAaSsDdFfGgHhJjKkLlZzXxCcVvBbNnMm";
        public static string Numbers { get; set; } = "1234567890";
        public static string LettersNumbers { get; set; } = "Qq1Ww2Ee3Rr4Tt5Yy6Uu7Ii8Oo9Pp0AaSsDdFfGgHhJjKkLlZzXxCcVvBbNnMm";

        public static string GetConfirmationToken(int length)
        {
            var RND = new Random(DateTime.Now.Millisecond);
            var SB = new StringBuilder();
            int i = 0;
            while (i < length)
            {
                SB.Append(Alphabet[RND.Next(Alphabet.Length)]);
                i++;
            }
            return SB.ToString();
        }

        /// <summary>
        /// Вернет строку пароля (буквы регистр цифры)
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetAccountPassword(int length)
        {
            var RND = new Random(DateTime.Now.Millisecond);
            var SB = new StringBuilder();
            int i = 0;
            while (i < length)
            {
                SB.Append(LettersNumbers[RND.Next(LettersNumbers.Length)]);
                i++;
            }
            return SB.ToString();
        }
    }
}
