using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaxtonSBI.DTO
{
    public class DiscoverDTO
    {
        public string deviceId { get; set; }
        public string deviceStatus { get; set; }
        public string certification { get; set; }
        public string serviceVersion { get; set; }
        public List<string> deviceSubId { get; set; }
        public string callbackId { get; set; }
        public string digitalId { get; set; }
        public string deviceCode { get; set; }
        public List<string> specVersion { get; set; }
        public string purpose { get; set; }
        public ErrorMsg error { get; set; }
    }
}
