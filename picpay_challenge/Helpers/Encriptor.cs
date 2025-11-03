using System.Security.Cryptography;
using System.Text;

namespace picpay_challenge.Helpers
{

    public static class Encriptor
    {
        private static readonly IConfigurationRoot _configuration;

        static Encriptor()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
        public static string Encrypt(string val)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_configuration["EncriptionKey"]); ;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(val);
                    return Convert.ToBase64String(encryptor.TransformFinalBlock(bytes, 0, bytes.Length));
                }
            }
        }

    }
}
