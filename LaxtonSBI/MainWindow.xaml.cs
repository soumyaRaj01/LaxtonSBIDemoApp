using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LaxtonSBI.API;
using LaxtonSBI.DTO;
using LaxtonSBI.Helper;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace LaxtonSBI
{
    public partial class MainWindow : Window
    {
        private int transactionId = 1111;
        Dictionary<string, DeviceInfoDTO> availableDeviceMap;
        Dictionary<string, byte[]> capturedImages;

        public MainWindow()
        {
            availableDeviceMap = new Dictionary<string, DeviceInfoDTO>();
            capturedImages = new Dictionary<string, byte[]>();

            
            getDeviceInfo();
            InitializeComponent();

            //InitSampleImage();
        }

        private void InitSampleImage()
        {
            
            string filePath = "D:\\LaxtonSBI\\LaxtonSBI\\SampleData\\face_response.txt";
            string fingerPath = "D:\\LaxtonSBI\\LaxtonSBI\\SampleData\\finger_response.txt";
            string irisPath = "D:\\LaxtonSBI\\LaxtonSBI\\SampleData\\iris_response.txt";


            string faceContent = File.ReadAllText(filePath);
            string fingerContent = File.ReadAllText(fingerPath);
            string irisContent = File.ReadAllText(irisPath);

            CaptureResponseDTO face = JsonConvert.DeserializeObject<CaptureResponseDTO>(faceContent);
            CaptureResponseDTO finger = JsonConvert.DeserializeObject<CaptureResponseDTO>(fingerContent);
            CaptureResponseDTO iris = JsonConvert.DeserializeObject<CaptureResponseDTO>(irisContent);

            List<CaptureResponseDTO> dto = new List<CaptureResponseDTO>();
            dto.Add(face);
            dto.Add(finger);
            dto.Add(iris);

            showSampleImage(dto);
        }

        public void showSampleImage(List<CaptureResponseDTO> dtos)
        {
            int cnt = 1;
            foreach(CaptureResponseDTO dto in dtos)
            {
                List<BiometricsDataDTO> biometricsData = new List<BiometricsDataDTO>();
                var jwtHelper = new JwtHelper();

                foreach (CaptureBiometricsDTO bioDto in dto.biometrics)
                {
                    BiometricsDataDTO bio = JsonConvert.DeserializeObject<BiometricsDataDTO>(jwtHelper.Decode(bioDto.data, false));
                    biometricsData.Add(bio);
                }

                if (cnt == 1) showCaptureImageAndScore(biometricsData, SBIConstants.FACE);
                else if(cnt ==2) showCaptureImageAndScore(biometricsData, SBIConstants.FINGERPRINT);
                else if(cnt == 3) showCaptureImageAndScore(biometricsData, SBIConstants.IRIS);
                cnt++;
            }
        }

        public async void getDeviceInfo()
        {
            //FeedbackMsg.Text = "Scanning for devices...";

            availableDeviceMap.Clear();

            // Device Info API call
            List<DeviceInfoDTO> infoResponse = await deviceInfoAPI();

            // Update DeviceInfoMap
            var jwtHelper = new JwtHelper();
            
            foreach (DeviceInfoDTO info in infoResponse)
            {
                var digitalId = JsonConvert.DeserializeObject<DigitalIdDTO>(jwtHelper.Decode(info.digitalId, false));

                string key = digitalId.type + "_" + digitalId.deviceSubType;

                availableDeviceMap.Add(key, info);
            }


            Console.WriteLine("Device Info Recieved");

            FeedbackMsg.Text = "Device Scan Completed!";
        }

        public static void DelayAction(int millisecond, Action action)
        {
            var timer = new DispatcherTimer();
            timer.Tick += delegate

            {
                action.Invoke();
                timer.Stop();
            };

            timer.Interval = TimeSpan.FromMilliseconds(millisecond);
            timer.Start();
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

        private void StreamAPI(string deviceId, string deviceSubId)
        {
            StreamAPI streamApi = new StreamAPI();
            StreamRequestDTO streamRequest = new StreamRequestDTO
            {
                DeviceId = deviceId,
                DeviceSubId = deviceSubId
            };

            Stream responseStream = streamApi.SendCustomRequestAsync(streamRequest);
            int cnt = 1;
            while (null != responseStream)
            {
                try
                {
                    var imageBytes = retrieveNextImage(responseStream);

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(imageBytes);
                    bitmapImage.EndInit();

                    StreamContent.Source = bitmapImage;

                    //ByteArrayInputStream imageStream = new ByteArrayInputStream(imageBytes);
                    //Image img = new Image(imageStream);
                    File.WriteAllBytes(@"D:\LaxtonSBI\LaxtonSBI\bin\Debug\bioutils\BiometricInfo\Face\" + "img_" + cnt + ".jpg", imageBytes);
                    cnt++;
                }
                catch (Exception t)
                {
                    responseStream = null;
                }
            }
        }

        public void ChangeImage(BitmapImage newImage)
        {
            // Set the source of the Image control to the provided BitmapImage
            FaceContent.Source = newImage;
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

        private async Task<List<BiometricsDataDTO>> CaptureAPI(int count, List<string> bioSubType, List<string> exception, DeviceInfoDTO info, DigitalIdDTO digitalInfo)
        {
            CaptureAPI captureApi = new CaptureAPI();
            CaptureRequestDTO captureRequest = new CaptureRequestDTO
            {
                env = SBIConstants.ENVIRONMENT_DEVELOPER,
                purpose = SBIConstants.PURPOSE_REGISTRATION,
                specVersion = info.specVersion.ToString(),
                timeout = 10000,
                captureTime = DateTime.UtcNow.ToString(SBIConstants.TimeStampFormat),
                transactionId = GetTransactionId(),
                bio = new List<CaptureRequestBIODTO>
                {
                    new CaptureRequestBIODTO
                    {
                        type = digitalInfo.type,
                        count = count.ToString(),
                        bioSubType = bioSubType,
                        exception = exception,
                        requestedScore = "40",
                        deviceId = info.deviceId,
                        deviceSubId = info.deviceSubId.ToString(),
                        previousHash = ""
                    }
                }
            };

            string apiResponse_string = await captureApi.SendCustomRequestAsync(captureRequest);

            CaptureResponseDTO response = JsonConvert.DeserializeObject<CaptureResponseDTO>(apiResponse_string);

            List<BiometricsDataDTO> biometricsData = new List<BiometricsDataDTO>();
            var jwtHelper = new JwtHelper();

            foreach (CaptureBiometricsDTO dto in response.biometrics)
            {
                BiometricsDataDTO bio = JsonConvert.DeserializeObject<BiometricsDataDTO>(jwtHelper.Decode(dto.data, false));
                biometricsData.Add(bio);
            }

            return biometricsData;
        }

        private void FaceCaptureBtn_Click(object sender, RoutedEventArgs e)
        {
            

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

                    StreamAPI(deviceId, deviceSubId);

                    // stream API
                    //_ = StreamAPI(deviceId, deviceSubId);

                    // Capture API
                    //Task rsp = collectDataAndCallAPIAsync(option, SBIConstants.FACE);
                }
                else
                {
                    FeedbackMsg.Text = "face device not available!";
                }
            }
        }

        private void FingerprintCaptureBtn_Click(object sender, RoutedEventArgs e)
        {
            

            ComboBoxItem item = (ComboBoxItem)FingerprintDropdown.SelectedItem;

            if (item == null)
            {
                FeedbackMsg.Text = "Select an option!";
            }
            else
            {
                string option = item.Content.ToString();

                if (isAvailable(SBIConstants.FINGERPRINT, SBIConstants.DEVICE_SUBTYPE_FINGER_SLAP))
                {
                    string key = SBIConstants.FINGERPRINT + "_" + SBIConstants.DEVICE_SUBTYPE_FINGER_SLAP;
                    string deviceId = availableDeviceMap[key].deviceId;

                    // stream API
                    //_ = StreamAPI(deviceId, deviceSubId);

                    // Capture API
                    //Task rsp = collectDataAndCallAPIAsync(option, SBIConstants.FINGERPRINT);
                    
                }
                else
                {
                    FeedbackMsg.Text = "fingerprint scannner not available!";
                }
            }

        }

        private void IrisCaptureBtn_Click(object sender, RoutedEventArgs e)
        {
            

            ComboBoxItem item = (ComboBoxItem)IrisDropdown.SelectedItem;

            if (item == null)
            {
                FeedbackMsg.Text = "Select an option!";
            }
            else
            {
                string option = item.Content.ToString();
                string deviceSubId = null;

                if (option == SBIConstants.ALL_IRISES_OPTION)
                {
                    deviceSubId = SBIConstants.DEVICE_SUBTYPE_IRIS_DOUBLE;
                }
                else if (option == SBIConstants.LEFT_IRIS_OPTION || option == SBIConstants.RIGHT_IRIS_OPTION)
                {
                    deviceSubId = SBIConstants.DEVICE_SUBTYPE_IRIS_SINGLE;
                }


                if (isAvailable(SBIConstants.IRIS, deviceSubId))
                {
                    string key = SBIConstants.IRIS + "_" + deviceSubId;
                    string deviceId = availableDeviceMap[key].deviceId;

                    Console.WriteLine(key);
                    // stream API
                    //_ = StreamAPI(deviceId, deviceSubId);

                    // Capture API
                    //Task rsp = collectDataAndCallAPIAsync(option, SBIConstants.IRIS);
                }
                else
                {
                    FeedbackMsg.Text = "iris scanner not available!";
                }
            }
        }

        private async Task collectDataAndCallAPIAsync(string deviceType, string option)
        {
            int count = 0;
            List<string> bioSubType = new List<string>();
            List<string> exception = new List<string>();
            string key = null;

            switch(deviceType)
            {
                case SBIConstants.FACE:
                    key = SBIConstants.FACE + "_" + SBIConstants.DEVICE_SUBTYPE_FACE_FULLFACE;

                    switch (option)
                    {
                        case SBIConstants.FACE_OPTION:
                            count = 1;
                            bioSubType.Add(SBIConstants.BIO_NAME_UNKNOWN);
                            break;
                    }
                    break;

                case SBIConstants.FINGERPRINT:
                    key = SBIConstants.FINGERPRINT + "_" + SBIConstants.DEVICE_SUBTYPE_FINGER_SLAP;
                    
                    switch (option)
                    {
                        case SBIConstants.LEFT_FINGERS_OPTION:
                            List<CheckBox> leftFingers = new List<CheckBox> {
                                LeftLittle_check,
                                LeftRing_check,
                                LeftMiddle_check,
                                LeftIndex_check
                            };

                            for (int i = 0; i < 4; i++)
                            {

                                if ((bool)leftFingers[i].IsChecked)
                                {
                                    count++;
                                    bioSubType.Add(SBIConstants.BIO_NAME_LEFT_FINGERS[i]);
                                }
                                else
                                {
                                    exception.Add(SBIConstants.BIO_NAME_LEFT_FINGERS[i]);
                                }
                            }
                            break;

                        case SBIConstants.RIGHT_FINGERS_OPTION:
                            List<CheckBox> rightFingers = new List<CheckBox> {
                                RightIndex_check,
                                RightMiddle_check,
                                RightRing_check,
                                RightLittle_check
                            };

                            for (int i = 0; i < 4; i++)
                            {

                                if ((bool)rightFingers[i].IsChecked)
                                {
                                    count++;
                                    bioSubType.Add(SBIConstants.BIO_NAME_RIGHT_FINGERS[i]);
                                }
                                else
                                {
                                    exception.Add(SBIConstants.BIO_NAME_RIGHT_FINGERS[i]);
                                }
                            }
                            break;

                        case SBIConstants.THUMBS_OPTION:
                            List<CheckBox> thumbs = new List<CheckBox> {
                                LeftThumb_check,
                                RightThumb_check
                            };

                            for (int i = 0; i < 2; i++)
                            {

                                if ((bool)thumbs[i].IsChecked)
                                {
                                    count++;
                                    bioSubType.Add(SBIConstants.BIO_NAME_THUMB[i]);
                                }
                                else
                                {
                                    exception.Add(SBIConstants.BIO_NAME_THUMB[i]);
                                }
                            }

                            break;
                    }
                    break;
                case SBIConstants.IRIS:

                    switch (option)
                    {
                        case SBIConstants.LEFT_IRIS_OPTION:
                            key = SBIConstants.IRIS + "_" + SBIConstants.DEVICE_SUBTYPE_IRIS_SINGLE;
                            if ((bool)LeftIris_check.IsChecked)
                            {
                                count++;
                                bioSubType.Add(SBIConstants.BIO_NAME_LEFT_IRIS);
                            }
                            else
                            {
                                exception.Add(SBIConstants.BIO_NAME_LEFT_IRIS);
                            }
                            break;

                        case SBIConstants.RIGHT_IRIS_OPTION:
                            key = SBIConstants.IRIS + "_" + SBIConstants.DEVICE_SUBTYPE_IRIS_SINGLE;
                            if ((bool)RightIris_check.IsChecked)
                            {
                                count++;
                                bioSubType.Add(SBIConstants.BIO_NAME_RIGHT_IRIS);
                            }
                            else
                            {
                                exception.Add(SBIConstants.BIO_NAME_RIGHT_IRIS);
                            }
                            break;

                        case SBIConstants.ALL_IRISES_OPTION:
                            key = SBIConstants.IRIS + "_" + SBIConstants.DEVICE_SUBTYPE_IRIS_DOUBLE;
                            if ((bool)LeftIris_check.IsChecked)
                            {
                                count++;
                                bioSubType.Add(SBIConstants.BIO_NAME_LEFT_IRIS);
                            }
                            else
                            {
                                exception.Add(SBIConstants.BIO_NAME_LEFT_IRIS);
                            }

                            if ((bool)RightIris_check.IsChecked)
                            {
                                count++;
                                bioSubType.Add(SBIConstants.BIO_NAME_RIGHT_IRIS);
                            }
                            else
                            {
                                exception.Add(SBIConstants.BIO_NAME_RIGHT_IRIS);
                            }
                            break;
                    }
                    break;
            }

            DeviceInfoDTO deviceInfo = availableDeviceMap[key];
            JwtHelper jwtHelper = new JwtHelper();
            var digitalId = JsonConvert.DeserializeObject<DigitalIdDTO>(jwtHelper.Decode(deviceInfo.digitalId));

            List<BiometricsDataDTO> captureResponse = await CaptureAPI(count, bioSubType, exception, deviceInfo, digitalId);

            showCaptureImageAndScore(captureResponse, deviceType);

        }

        private void showCaptureImageAndScore (List<BiometricsDataDTO> biometricsDTO, string type)
        {
            Dictionary<string, BitmapImage> ImagesToShow = new Dictionary<string, BitmapImage>();
            Dictionary<string, string> ScoresToShow = new Dictionary<string, string>();

            foreach(BiometricsDataDTO bio in biometricsDTO)
            {
                byte[] isoImage = Base64UrlEncoder.DecodeBytes(bio.bioValue);
                byte[] img = ImageHelper.ISOtoBytes(isoImage, type);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(img);
                bitmapImage.EndInit();

                if (type == SBIConstants.FACE)
                {
                    ImagesToShow.Add("FACE", bitmapImage);
                    capturedImages.Add("FACE", img);
                }
                else
                {
                    ImagesToShow.Add(bio.bioSubType, bitmapImage);
                    capturedImages.Add(bio.bioSubType, img);
                }
                ScoresToShow.Add(bio.bioSubType, bio.qualityScore);
            }
            

            switch(type)
            {
                case SBIConstants.FACE:
                    FaceContent.Source = ImagesToShow["FACE"];
                    break;

                case SBIConstants.FINGERPRINT:
                    foreach(KeyValuePair<string, BitmapImage> entry in ImagesToShow)
                    {
                        switch(entry.Key)
                        {
                            case SBIConstants.BIO_NAME_LEFT_LITTLE:
                                LeftLittle_img.Source = entry.Value;
                                LeftLittle_score.Text = ScoresToShow[entry.Key];
                                break;
                            case SBIConstants.BIO_NAME_LEFT_RING:
                                LeftRing_img.Source = entry.Value;
                                break;
                            case SBIConstants.BIO_NAME_LEFT_MIDDLE:
                                LeftMiddle_img.Source = entry.Value;
                                break;
                            case SBIConstants.BIO_NAME_LEFT_INDEX:
                                LeftIndex_img.Source = entry.Value;
                                break;
                            case SBIConstants.BIO_NAME_LEFT_THUMB:
                                LeftThumb_img.Source = entry.Value;
                                break;
                            case SBIConstants.BIO_NAME_RIGHT_THUMB:
                                RightThumb_img.Source = entry.Value;
                                break;
                            case SBIConstants.BIO_NAME_RIGHT_INDEX:
                                RightIndex_img.Source = entry.Value;
                                break;
                            case SBIConstants.BIO_NAME_RIGHT_MIDDLE:
                                RightMiddle_img.Source = entry.Value;
                                break;
                            case SBIConstants.BIO_NAME_RIGHT_RING:
                                RightRing_img.Source = entry.Value;
                                break;
                            case SBIConstants.BIO_NAME_RIGHT_LITTLE:
                                RightLittle_img.Source = entry.Value;
                                break;
                        }
                    }
                    break;

                case SBIConstants.IRIS:
                    foreach (KeyValuePair<string, BitmapImage> entry in ImagesToShow)
                    {
                        switch(entry.Key)
                        {
                            case SBIConstants.BIO_NAME_LEFT_IRIS:
                                LeftIris_img.Source = entry.Value;
                                break;
                            case SBIConstants.BIO_NAME_RIGHT_IRIS:
                                RightIris_img.Source = entry.Value;
                                break;
                        }
                    }
                    break;
            }
        }

        private string GetTransactionId()
        {
            return transactionId++.ToString();
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

        private void DeviceScanBtn_Click(object sender, RoutedEventArgs e)
        {
           
            getDeviceInfo();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            //foreach(KeyValuePair<string, byte[]> entry in capturedImages)
            //{
            //    string downloadsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";

            //    // Combine the Downloads folder path with the file name
            //    //string filePath = Path.Combine(downloadsFolderPath, img_);

            //    // Write the byte array to the file
            //    //File.WriteAllBytes(filePath, imageBytes);
            //}

        }
    }
}
