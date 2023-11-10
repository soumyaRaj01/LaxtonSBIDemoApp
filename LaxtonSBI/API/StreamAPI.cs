using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        public Stream SendCustomRequestAsync(StreamRequestDTO streamRequest)
        {
            try
            {
                string jsonRequest = JsonConvert.SerializeObject(streamRequest);
                //var client = new RestClient("http://localhost:4503/stream");
                //var request = new RestRequest();
                //request.AddBody(jsonRequest);
                //request.AddHeader("Accept", "*/*");
                //request.AddHeader("Expect", "");
                //request.AddHeader("Expect-continue", null);

                //var response = client.Post(request);
                //var content = response.Content; // Raw content as string
                //HttpContent content = new StringContent(jsonRequest);
                //client.DefaultRequestHeaders.ConnectionClose = false;

                //HttpResponseMessage response = client.PostAsync("/stream", new StringContent(jsonRequest, Encoding.UTF8, "text/plain")).Result;

                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:4503/stream");
                myHttpWebRequest.Method = "STREAM";

                byte[] data = Encoding.ASCII.GetBytes(jsonRequest);

                myHttpWebRequest.ContentType = "text/plain";
                //myHttpWebRequest.ContentLength = data.Length;
                //myHttpWebRequest.ProtocolVersion = HttpVersion.Version10;
                //myHttpWebRequest.KeepAlive = true;
                //myHttpWebRequest.ServicePoint.Expect100Continue = false;
                //myHttpWebRequest.Accept = "*/*";
                //myHttpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                Stream requestStream = myHttpWebRequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = myHttpWebResponse.GetResponseStream();
                    //StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

                    //string pageContent = myStreamReader.ReadToEnd();
                    int cnt = 1;
                    while (null != responseStream)
                    {
                        try
                        {
                            var imageBytes = retrieveNextImage(responseStream);
                            //ByteArrayInputStream imageStream = new ByteArrayInputStream(imageBytes);
                            //Image img = new Image(imageStream);
                            File.WriteAllBytes(@"D:\LaxtonRepo\LaxtonUILatest\LaxtonSBIDemoApp\LaxtonSBI\bin\Debug\bioutils\BiometricInfo\Face\" + "img_" + cnt + ".jpg", imageBytes);
                            cnt++;
                        }
                        catch (Exception t)
                        {
                            responseStream = null;
                        }
                    }

                    return null;
                }
                else
                {
                    // Handle the error response
                    Console.WriteLine($"HTTP request failed with status code {myHttpWebResponse.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle any HTTP request exceptions
                Console.WriteLine($"HTTP request failed: {ex.Message}");
            }

            return null;
        }

        public byte[] retrieveNextImage(Stream urlStream)
        {
            try
            {
                int currByte = -1;

                bool captureContentLength = false;
                StringBuilder contentStringBuilder = new StringBuilder(128);
                StringBuilder headerStringBuilder = new StringBuilder(128);
                StringWriter contentLengthStringWriter = new StringWriter(contentStringBuilder);
                StringWriter headerWriter = new StringWriter(headerStringBuilder);

                int contentLength = 0;

                while ((currByte = urlStream.ReadByte()) > -1)
                {
                    if (captureContentLength)
                    {
                        if (currByte == 10 || currByte == 13)
                        {
                            contentLength = int.Parse(contentStringBuilder.ToString().Replace(" ", ""));
                            break;
                        }
                        contentLengthStringWriter.Write((char)currByte);

                    }
                    else
                    {
                        headerWriter.Write((char)currByte);
                        string tempString = headerStringBuilder.ToString();
                        int indexOf = tempString.IndexOf("Content-Length:");
                        if (indexOf > 0)
                        {
                            captureContentLength = true;
                        }
                    }
                }

                // 255 indicates the start of the jpeg image
                while (urlStream.ReadByte() != 255)
                {

                }

                // && urlStream.read()!=-1
                // if(urlStream.read()==-1) {
                // throw new RuntimeException("No stream available");
                // }

                // rest is the buffer
                byte[] imageBytes = new byte[contentLength + 1];
                // since we ate the original 255 , shove it back in
                imageBytes[0] = (byte)255;
                int offset = 1;
                int numRead = 0;
                while (offset < imageBytes.Length
                        && (numRead = urlStream.Read(imageBytes, offset, imageBytes.Length - offset)) >= 0)
                {
                    offset += numRead;
                }

                return imageBytes;
            }
            catch
            {
                throw;
            }
        }
    }
}
