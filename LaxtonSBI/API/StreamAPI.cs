using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LaxtonSBI.DTO;
using Newtonsoft.Json;

namespace LaxtonSBI.API
{
    public class StreamAPI
    {
        private readonly HttpClient client;
        private HttpContent content;

        public StreamAPI()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:4503");

            //string jsonBody = "{\"deviceId\": \"511\", \"deviceSubId\": \"1\", \"timeout\": \"2000\"}";
            //content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        public async Task<Stream> SendCustomRequestAsync(StreamRequestDTO streamRequest)
        {
            try
            {
                // Serialize the StreamRequestDTO object to JSON
                string jsonRequest = JsonConvert.SerializeObject(streamRequest);

                // Send a POST request to the stream endpoint
                HttpResponseMessage response = await client.PostAsync("/stream", new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    Stream responseStream = await response.Content.ReadAsStreamAsync();
                    return responseStream;
                }
                else
                {
                    // Handle the error response
                    Console.WriteLine($"HTTP request failed with status code {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle any HTTP request exceptions
                Console.WriteLine($"HTTP request failed: {ex.Message}");
            }

            return null;
        }
    }
}
