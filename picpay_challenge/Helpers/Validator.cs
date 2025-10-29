using System.Text.RegularExpressions;

namespace picpay_challenge.Helpers
{
    public static class Validator
    {
        public static bool IsValidCPF(string CPF)
        {
            return Regex.IsMatch(CPF, @"[0-9]{3}\.?[0-9]{3}\.?[0-9]{3}\-?[0-9]{2}");
        }
        public static bool IsValidEmail(string Email)
        {
            return Regex.IsMatch(Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        }
        public static bool IsValidCNPJ(string? CNPJ)
        {
            return CNPJ != null ?
                    Regex.IsMatch(CNPJ, @"[0-9]{2}\.?[0-9]{3}\.?[0-9]{3}\/?[0-9]{4}\-?[0-9]{2}")
                    : true;
        }

    }
}
