using picpay_challenge.Domain.DTOs.PaymentExternalAuthorizorDTOs;
using System.Net.Http;
using System.Text.Json;

namespace picpay_challenge.Domain.Integrations
{
    public class PaymentExternalAuthorizor
    {
        private readonly HttpClient _httpClient;
        public PaymentExternalAuthorizor(HttpClient httpsClient)
        {
            _httpClient = httpsClient;
        }
        public async Task<PaymentAuthorizationDTO?> IsPaymentAuthorized()
        {
            try
            {
                HttpResponseMessage res = await _httpClient.GetAsync("https://util.devi.tools/api/v2/authorize");
                string resJSON = await res.Content.ReadAsStringAsync();
                Console.WriteLine(res);
                var paymentAuthorization = JsonSerializer.Deserialize<PaymentAuthorizationDTO>
                    (resJSON, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? null;
                return paymentAuthorization ?? throw new Exception("Failed to deserialize payment authorization response");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

    }
}
