using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;


namespace LaxtonSBI.API
{
    public class DiscoverAPI
    {
        private readonly HttpClient client;
        private HttpContent content;

        public DiscoverAPI ()
        {
            client = new HttpClient();

            string jsonBody = "{ \"type\": \"Biometric Device\" }";
            content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        public async Task<string> SendCustomRequestAsync()
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("MOSIPDISC"), "http://localhost:4503/device"))
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
