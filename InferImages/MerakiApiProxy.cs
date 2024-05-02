using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InferImages
{
    internal class MerakiAPIProxy(IHttpClientFactory httpClientFactory)
    {
        private readonly string Token = "aa590dd9f73fc83ea5c88072aa7835e431fba4c5";
        private readonly string Murl = "https://api.meraki.com/api/v1/devices/{0}/camera/generateSnapshot";        
        private readonly HttpClient httpClientWithRetry = httpClientFactory.CreateClient("HttpClientWithRetry");
        private static int UrlReqCount = 0;
        internal async ValueTask<string> GetImageUrl<T>(T payload, string camSerial)
        {
            httpClientWithRetry.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer", Token);

            httpClientWithRetry.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonPayload = JsonSerializer.Serialize(payload);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var url = string.Format(Murl, camSerial);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var response = await httpClientWithRetry.PostAsync(url, content);


            if (response.IsSuccessStatusCode)
            {
                stopwatch.Stop();
                Console.WriteLine($"POST request {camSerial} took {stopwatch.ElapsedMilliseconds} ms.");
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                stopwatch.Stop();
                return "Error retrieving url";
            }


            
            /*var errorMessage = await BuildErrorMessage(response);
            errorMessage += $"Payload: {jsonPayload}";

            throw new Exception(errorMessage);*/
        }

        internal async ValueTask<Stream> GetImage(string url, string camSerial)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var response = await httpClientWithRetry.GetAsync(url);

            stopwatch.Stop();
            Console.WriteLine($"GET request {camSerial} took {stopwatch.ElapsedMilliseconds} ms.");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStreamAsync();
            }
            else
            {
                Console.WriteLine("Error retrieving image");
                return null;
            }

            //throw new Exception(await BuildErrorMessage(response));
        }

        private static async Task<string> BuildErrorMessage(HttpResponseMessage response)
        {
            var statusCode = response.StatusCode;
            var reasonPhrase = response.ReasonPhrase;
            var content = await response.Content.ReadAsStringAsync();

            return new StringBuilder()
                .AppendLine($"Status Code: {statusCode}")
                .AppendLine($"Reason Phrase: {reasonPhrase}")
                .AppendLine($"Content: {content}")
                .ToString();
        }
    }
}
