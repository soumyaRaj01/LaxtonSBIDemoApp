using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LaxtonSBI.Helper;

namespace LaxtonSBI.API
{
    public class DeviceInfoAPI
    {
        private readonly HttpClient client;
        private string URI;

        public DeviceInfoAPI()
        {
            client = new HttpClient();
            URI = SBIConstants.BASE_URI + SBIConstants.INFO;
        }

        public async Task<string> SendCustomRequestAsync()
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(SBIConstants.MOSIP_METHOD_MOSIPDINFO), URI))
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
