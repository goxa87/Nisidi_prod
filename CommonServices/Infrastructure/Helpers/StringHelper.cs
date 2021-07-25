using System;
using System.Collections.Generic;
using System.Text;

namespace CommonServices.Infrastructure.Helpers
{
    public static class StringHelper
    {
        public static string Alphabet { get; set; } = "QqWwEeRrTtYyUuIiOoPpAaSsDdFfGgHhJjKkLlZzXxCcVvBbNnMm1234567890!@#$%^&*()=+-_";
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
    }
}
