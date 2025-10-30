using System.Text.RegularExpressions;

namespace picpay_challenge.Helpers
{
    public static class Validator
    {
        public static bool IsValidCPF(string CPF)
        {
            string sanitized = Sanitizer.OnlyDigits(CPF);
            if (sanitized.Length != 11 ||
                sanitized == "00000000000" ||
                sanitized == "11111111111" ||
                sanitized == "22222222222" ||
                sanitized == "33333333333" ||
                sanitized == "44444444444" ||
                sanitized == "55555555555" ||
                sanitized == "66666666666" ||
                sanitized == "77777777777" ||
                sanitized == "88888888888" ||
                sanitized == "99999999999"
                ) return false;
            int add = 0;
            for (int i = 0; i < 9; i++) add += sanitized[i] * (10 - i);

            int validator = 11 - (add % 11);
            if (validator == 10 || validator == 11) validator = 0;

            add = 0;
            for (int i = 0; i < 10; i++) add += sanitized[i] * (11 - i);
            validator = 11 - (add % 11);
            if (validator == 10 || validator == 11) validator = 0;
            if (validator != sanitized[10]) return false;
            return true;

        }
        public static bool IsValidEmail(string Email)
        {
            return Regex.IsMatch(Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        }
        public static bool IsValidCNPJ(string? CNPJ)
        {

            string sanitized = Sanitizer.OnlyDigits(CNPJ);

            if (CNPJ.Length != 14 ||
               CNPJ == "00000000000000" ||
               CNPJ == "11111111111111" ||
               CNPJ == "22222222222222" ||
               CNPJ == "33333333333333" ||
               CNPJ == "44444444444444" ||
               CNPJ == "55555555555555" ||
               CNPJ == "66666666666666" ||
               CNPJ == "77777777777777" ||
               CNPJ == "88888888888888" ||
               CNPJ == "99999999999999"
                )
                return false;
            int size = sanitized.Length - 2;
            int sum = 0;
            string numbers = sanitized.Substring(0, size);
            string digits = sanitized.Substring(size);
            int pos = size - 7;
            for (int i = size; i >= 0; i--)
            {
                sum += numbers[i] * pos--;
                if (pos < 2)
                {
                    pos = 9;
                }
            }
            var result = sum % 11 < 2 ? 0 : 11 - sum % 11;
            if (result !== digits[0]) return false;

            size++;
            numbers = sanitized.Substring(0, size);
            sum = 0;
            pos = size - 7;
            for (int i = size; i >= 0; i--)
            {
                sum += numbers[size - i] * pos--;
                if (pos < 2) pos = 9;
            }
            result = sum % 11 < 2 ? 0 : 11 - sum % 11;
            if (result != digits[1]) return false;
            return true;
        }

    }
}
