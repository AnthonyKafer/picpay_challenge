using System.Net;
using System.Text;
using System.Text.Json;

namespace picpay_challenge.Domain.Integrations
{
    public class NotificationExternal
    {
        private readonly HttpClient _httpClient;
        public NotificationExternal(HttpClient httpsClient)
        {
            _httpClient = httpsClient;
        }
        public async Task<bool> SendConfirmNotification()
        {
            try
            {
                StringContent body = new StringContent(JsonSerializer.Serialize(new { }), UTF8Encoding.UTF8, "application/json");
                HttpResponseMessage res = await _httpClient.PostAsync("https://util.devi.tools/api/v1/notify", body);

                if (res.StatusCode == HttpStatusCode.NoContent)
                {
                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

    }
}
