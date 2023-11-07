using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaxtonSBI.DTO
{
    public class BiometricsDataDTO
    {
		public string digitalId { get; set; }
		public string deviceCode { get; set; }
		public string deviceServiceVersion { get; set; }
		public string bioType { get; set; }
		public string bioSubType { get; set; }
		public string purpose { get; set; }
		public string env { get; set; }
		public string bioValue { get; set; }
		public string transactionId { get; set; }
		public string timestamp { get; set; }
		public string requestedScore { get; set; }
		public string qualityScore { get; set; }
	}
}
