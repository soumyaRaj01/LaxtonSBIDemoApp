using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaxtonSBI.DTO
{
    public class DeviceInfoDTO
    {
        public string callbackId { get; set; }
        public List<string> specVersion { get; set; }
        public string env { get; set; }
        public string digitalId { get; set; }
        public string deviceId { get; set; }
        public string deviceCode { get; set; }
        public string purpose { get; set; }
        public string serviceVersion { get; set; }
        public string deviceStatus { get; set; }
        public string firmware { get; set; }
        public string certification { get; set; }
        public List<string> deviceSubId { get; set; }
    }

    public class DigitalIdDTO
    {
        public string serialNo { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string type { get; set; }
        public string deviceSubType { get; set; }
        public string deviceProviderId { get; set; }
        public string deviceProvider { get; set; }
        public string dateTime { get; set; }
    }
}
