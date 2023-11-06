using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LaxtonSBI.DTO;
using Newtonsoft.Json;
using LaxtonSBI.Helper;

namespace LaxtonSBI.API
{
    public class CaptureAPI
    {
        private readonly HttpClient client;
        private HttpContent content;
        private string URI;

        public CaptureAPI()
        {
            client = new HttpClient();
            URI = SBIConstants.BASE_URI + SBIConstants.CAPTURE;
        }

        public async Task<string> SendCustomRequestAsync(CaptureRequestDTO captureRequest)
        {
            string jsonRequest = JsonConvert.SerializeObject(captureRequest);
            content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(SBIConstants.MOSIP_METHOD_CAPTURE), URI))
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
