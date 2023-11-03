using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LaxtonSBI.API
{
    public class CaptureAPI
    {
        private readonly HttpClient client;
        private HttpContent content;

        public CaptureAPI()
        {
            client = new HttpClient();

            string jsonBody = "{}";
            content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        public async Task<string> SendCustomRequestAsync()
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("CAPTURE"), "http://localhost:4503/capture"))
            {
                request.Content = content;
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                else
                {
                    throw new HttpRequestException($"HTTP request failed with status code {response.StatusCode}");
                }
            }
        }
    }
}
