using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LaxtonSBI.API;
using LaxtonSBI.DTO;
using LaxtonSBI.Helper;
using Newtonsoft.Json;

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
            List<DeviceInfoDTO> infoResponse = await deviceInfoAPI();

            // Update DeviceInfoMap
            var jwtHelper = new JwtHelper();
            foreach (DeviceInfoDTO info in infoResponse)
            {
                var digitalId = JsonConvert.DeserializeObject<DigitalIdDTO>(jwtHelper.Decode(info.digitalId, false));

                string key = digitalId.type + "_" + digitalId.deviceSubType;

                availableDeviceMap.Add(key, info);

                Console.WriteLine(key);
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

        public async Task<List<DeviceInfoDTO>> deviceInfoAPI()
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


        private void FaceCaptureBtn_Click(object sender, RoutedEventArgs e)
        {
            FeedbackMsg.Text = "Feedback messages to the user is displayed here";

            ComboBoxItem item = (ComboBoxItem)FaceDropdown.SelectedItem;

            if (item == null)
            {
                FeedbackMsg.Text = "Select an option!";
            }
            else
            {
                string option = item.Content.ToString();
                string deviceSubId = null;

                if (option == "Capture face")
                {
                    deviceSubId = SBIConstants.DEVICE_SUBTYPE_FACE_FULLFACE;
                }


                if (isAvailable(SBIConstants.FACE, deviceSubId))
                {
                    // Device Info
                    string key = SBIConstants.FACE + "_" + deviceSubId;
                    string deviceId = availableDeviceMap[key].deviceId;
                    Console.WriteLine(key);


                    // capture API
                    //_ = CaptureAPI();

                    // stream API
                    //_ = StreamAPI(deviceId, deviceSubId);

                }
                else
                {
                    FeedbackMsg.Text = "face device not available!";
                }
            }
        }

        private void FingerprintCaptureBtn_Click(object sender, RoutedEventArgs e)
        {
            FeedbackMsg.Text = "Feedback messages to the user is displayed here";

            ComboBoxItem item = (ComboBoxItem)FingerprintDropdown.SelectedItem;
            
            if(item == null)
            {
                FeedbackMsg.Text = "Select an option!";
            }
            else
            {
                string option = item.Content.ToString();
                string deviceSubId = null;

                if(option == SBIConstants.ALL_FINGERS)
                {
                    deviceSubId = SBIConstants.DEVICE_SUBTYPE_FINGER_SLAP;
                }


                if (isAvailable(SBIConstants.FINGERPRINT, deviceSubId))
                {
                    string key = SBIConstants.FINGERPRINT + "_" + deviceSubId;
                    string deviceId = availableDeviceMap[key].deviceId;


                    // capture API
                    //_ = CaptureAPI();

                    // stream API
                    //_ = StreamAPI(deviceId, deviceSubId);
                }
                else
                {
                    FeedbackMsg.Text = "fingerprint scannner not available!";
                }
            }
            
        }

        private void IrisCaptureBtn_Click(object sender, RoutedEventArgs e)
        {
            FeedbackMsg.Text = "Feedback messages to the user is displayed here";

            ComboBoxItem item = (ComboBoxItem)IrisDropdown.SelectedItem;

            if (item == null)
            {
                FeedbackMsg.Text = "Select an option!";
            }
            else
            {
                string option = item.Content.ToString();
                string deviceSubId = null;

                if (option == SBIConstants.ALL_IRISES)
                {
                    deviceSubId = SBIConstants.DEVICE_SUBTYPE_IRIS_DOUBLE;
                }
                else if (option == SBIConstants.LEFT_IRIS || option == SBIConstants.RIGHT_IRIS)
                {
                    deviceSubId = SBIConstants.DEVICE_SUBTYPE_IRIS_SINGLE;
                }


                if (isAvailable(SBIConstants.IRIS, deviceSubId))
                {
                    string key = SBIConstants.IRIS + "_" + deviceSubId;
                    string deviceId = availableDeviceMap[key].deviceId;

                    Console.WriteLine(key);
                    // capture API
                    //_ = CaptureAPI();

                    // stream API
                    //_ = StreamAPI(deviceId, deviceSubId);
                }
                else
                {
                    FeedbackMsg.Text = "iris scanner not available!";
                }
            }
        }

        private async Task StreamAPI(string deviceId, string deviceSubId)
        {
            StreamAPI streamApi = new StreamAPI();
            StreamRequestDTO streamRequest = new StreamRequestDTO
            {
                DeviceId = deviceId,
                DeviceSubId = deviceSubId,
                Timeout = "2000"
            };

            Stream responseStream = await streamApi.SendCustomRequestAsync(streamRequest);
            if (responseStream != null)
            {
                Console.WriteLine("originalData: " + responseStream);
                Console.WriteLine("Stream Request Completed");


            }
        }

        private async Task<List<CaptureBiometricsDTO>> CaptureAPI()
        {
            CaptureAPI captureApi = new CaptureAPI();
            CaptureRequestDTO captureRequest = new CaptureRequestDTO
            {
                // create request body
            };

            string apiResponse_string = await captureApi.SendCustomRequestAsync(captureRequest);

            List<CaptureBiometricsDTO> response = JsonConvert.DeserializeObject<List<CaptureBiometricsDTO>>(apiResponse_string);

            return response;
        }

        private bool isAvailable(string deviceType, string subType)
        {
            bool available = false;
            string key = deviceType + "_" + subType;

            if (availableDeviceMap.ContainsKey(key) == true)
            {
                available = true;
            }

            return available;
        }


    }
}
