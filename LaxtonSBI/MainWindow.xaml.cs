using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LaxtonSBI.API;
using LaxtonSBI.DTO;
using Newtonsoft.Json;
using LaxtonSBI.Helper;
using Microsoft.IdentityModel.Tokens;
using System.IO;

namespace LaxtonSBI
{
    public partial class MainWindow : Window
    {

        Dictionary<string, DeviceInfoDTO> availableDeviceMap;

        public MainWindow()
        {
            availableDeviceMap = new Dictionary<string, DeviceInfoDTO>();

            getDeviceInfo();
            InitializeComponent();
            
        }

        public async void getDeviceInfo()
        {

            // Device Info API call
            List<DeviceInfoDTO> infoResponse = await deviceInfo();



            // Update DeviceInfoMap'
            var jwtHelper = new JwtHelper();
            foreach (DeviceInfoDTO info in infoResponse)
            {

                //byte[] digitalIdBytes = Base64UrlEncoder.DecodeBytes(info.digitalId);
                //string digitalIdJson = Encoding.UTF8.GetString(digitalIdBytes);

                var digitalId = JsonConvert.DeserializeObject<DigitalIdDTO>(jwtHelper.Decode(info.digitalId, false));

                string key = digitalId.type + "_" + digitalId.deviceSubType;

                availableDeviceMap.Add(key, info);

                //Console.WriteLine(key);
                //Console.WriteLine("Device Info:");
                //Console.WriteLine($"CallbackId: {info.callbackId}");
                //Console.WriteLine($"SpecVersion: {string.Join(", ", info.specVersion)}");
                //Console.WriteLine($"Env: {info.env}");
                //Console.WriteLine($"DigitalId: {info.digitalId}");
                //Console.WriteLine($"DeviceId: {info.deviceId}");
                //Console.WriteLine($"DeviceCode: {info.deviceCode}");
                //Console.WriteLine($"Purpose: {info.purpose}");
                //Console.WriteLine($"ServiceVersion: {info.serviceVersion}");
                //Console.WriteLine($"DeviceStatus: {info.deviceStatus}");
                //Console.WriteLine($"Firmware: {info.firmware}");
                //Console.WriteLine($"Certification: {info.certification}");
                //Console.WriteLine($"DeviceSubId: {string.Join(", ", info.deviceSubId)}");
            }


            Console.WriteLine("Device Info Recieved");
            FeedbackMsg.Text = "Feedback messages to the user is displayed here";
        }

        public async Task<List<DeviceInfoDTO>> deviceInfo()
        {
        
            DeviceInfoAPI deviceInfoAPI = new DeviceInfoAPI();
            var jwtHelper = new JwtHelper();

            string apiResponse_string = await deviceInfoAPI.SendCustomRequestAsync();

            List<DeviceInfoResponseDTO> _response = JsonConvert.DeserializeObject<List<DeviceInfoResponseDTO>>(apiResponse_string);
            List<DeviceInfoDTO> response = new List<DeviceInfoDTO>();

            //decode _response into Lst<DeviceInfoDTO>
            for (int idx = 0; idx < _response.ToList().Count; idx++)
            {
                var response_idx = JsonConvert.DeserializeObject<DeviceInfoDTO>(jwtHelper.Decode(_response[idx].deviceInfo, false));
                response.Add(response_idx);
            }

            return response;
        }

        private void FingerprintCaptureBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isAvailable(SBIConstants.FACE))
            {
                // capture API
                //CaptureAPI capture = new CaptureAPI();

                // stream API
                //StreamAPI stream = new StreamAPI();
                //var response = stream.SendCustomRequestAsync();

                //Console.WriteLine("Stream response: " + response);
                //Console.WriteLine("Stream response string: " + response.ToString());
                //Console.WriteLine("Stream response: " + response.Result);
                //Console.WriteLine("Stream response: " + response.IsCompleted);
                //Console.WriteLine("Stream response: " + response.Status);

                StreamAPIAsync();
                
            }
        }

        private async Task StreamAPIAsync()
        {
            StreamAPI streamApi = new StreamAPI();
            StreamRequestDTO streamRequest = new StreamRequestDTO
            {
                DeviceId = "511",
                DeviceSubId = "1",
                Timeout = "2000"
            };

            Stream responseStream = await streamApi.SendCustomRequestAsync(streamRequest);
            if (responseStream != null)
            {
                // Handle the response stream as needed
                // For example, you can read and process the stream data here
                Console.WriteLine("originalData: " + responseStream);
                Console.WriteLine("stringData: " + responseStream.);
                Console.WriteLine("Stream Request Completed");

                
            }
        }


        private bool isAvailable(string v)
        {
            bool available = true;

            if (v == SBIConstants.FINGERPRINT)
            {

            }
            else if (v == SBIConstants.FACE)
            {

            }
            else if (v == SBIConstants.IRIS)
            {

            }

            return available;
        }

        private void FaceCaptureBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
