using System.Text.RegularExpressions;

namespace picpay_challenge.Helpers
{
    public static class Validator
    {
        public static bool IsValidCPF(string CPF)
        {
            string sanitized = Sanitizer.OnlyDigits(CPF);

            if (sanitized.Length != 11 ||
                sanitized.All(c => c == sanitized[0])) return false;

            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += (sanitized[i] - '0') * (10 - i);

            int firstDigit = 11 - (sum % 11);
            if (firstDigit >= 10) firstDigit = 0;

            if (firstDigit != (sanitized[9] - '0')) return false;

            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += (sanitized[i] - '0') * (11 - i);

            int secondDigit = 11 - (sum % 11);
            if (secondDigit >= 10) secondDigit = 0;

            return secondDigit == (sanitized[10] - '0');

        }
        public static bool IsValidEmail(string Email)
        {
            return Regex.IsMatch(Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        }
        public static bool IsValidCNPJ(string CNPJ)
        {

            string sanitized = Sanitizer.OnlyDigits(CNPJ);

            if (sanitized.Length != 14 ||
               sanitized == "00000000000000" ||
               sanitized == "11111111111111" ||
               sanitized == "22222222222222" ||
               sanitized == "33333333333333" ||
               sanitized == "44444444444444" ||
               sanitized == "55555555555555" ||
               sanitized == "66666666666666" ||
               sanitized == "77777777777777" ||
               sanitized == "88888888888888" ||
               sanitized == "99999999999999"
                )
                return false;


            int size = sanitized.Length - 2;
            int sum = 0;
            string numbers = sanitized.Substring(0, size);
            string digits = sanitized.Substring(size);
            int pos = size - 7;


            for (int i = size; i >= 1; i--)
            {
                sum += (numbers[size - i] - '0') * pos--;
                if (pos < 2) pos = 9;
            }
            var result = sum % 11 < 2 ? 0 : 11 - sum % 11;
            if (result != (digits[0] - '0')) return false;

            size++;
            numbers = sanitized.Substring(0, size);
            sum = 0;
            pos = size - 7;

            for (int i = size; i >= 1; i--)
            {
                sum += (numbers[size - i] - '0') * pos--;
                if (pos < 2) pos = 9;
            }
            result = sum % 11 < 2 ? 0 : 11 - sum % 11;
            if (result != (digits[1] - '0')) return false;
            return true;
        }

    }
}
