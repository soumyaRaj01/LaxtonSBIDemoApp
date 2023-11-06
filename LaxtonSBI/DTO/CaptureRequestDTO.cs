using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaxtonSBI.DTO
{
    public class CaptureRequestDTO
    {
		public string env { get; set; }
		public string purpose { get; set; }
		public string specVersion { get; set; }
		public int timeout { get; set; }
		public string domainUri { get; set; }
		public string captureTime { get; set; }
		public string transactionId { get; set; }

		public List<CaptureRequestBIODTO> bio { get; set; }
	}

    public class CaptureRequestBIODTO
    {
        public string type { get; set; }
        public string count { get; set; }
        public List<string> exception { get; set; }
        public List<string> bioSubType { get; set; }
        public string requestedScore { get; set; }
        public string deviceId { get; set; }
        public string deviceSubId { get; set; }
        public string previousHash { get; set; }
    }
}
