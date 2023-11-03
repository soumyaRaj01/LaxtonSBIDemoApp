using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaxtonSBI.DTO
{
    public class StreamRequestDTO
    {
        public string DeviceId { get; set; }
        public string DeviceSubId { get; set; }
        public string Timeout { get; set; }
    }
}
