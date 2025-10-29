using System.Text.RegularExpressions;

namespace picpay_challenge.Helpers
{
    public static class Sanitizer
    {
        public static string OnlyDigits(string input)
        {
            return Regex.Replace(input, @"[^\d]", "");
        }
        public static string OnlyLetters(string input)
        {
            return Regex.Replace(input, @"[^a-zA-Z]", "");
        }
    }
}
