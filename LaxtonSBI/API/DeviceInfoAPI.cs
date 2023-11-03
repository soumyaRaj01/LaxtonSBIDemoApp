using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LaxtonSBI.API
{
    public class DeviceInfoAPI
    {
        private readonly HttpClient client;

        public DeviceInfoAPI()
        {
            client = new HttpClient();
        }

        public async Task<string> SendCustomRequestAsync()
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("MOSIPDINFO"), "http://localhost:4503/info"))
            {
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
