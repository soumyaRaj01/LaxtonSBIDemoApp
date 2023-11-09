using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LaxtonSBI.Helper
{
    public class JwtHelper
    {	
		public string Decode(string token, bool verify = true)
		{
			string[] parts = token.Split('.');
			string payload = parts[1];

			string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
			JObject payloadData = JObject.Parse(payloadJson);

			return payloadData.ToString();
		}

		private static byte[] Base64UrlDecode(string input)
		{
			var output = input;
			output = output.Replace('-', '+'); // 62nd char of encoding
			output = output.Replace('_', '/'); // 63rd char of encoding

			switch (output.Length % 4) // Pad with trailing '='s
			{
				case 0: break; // No pad chars in this case
				case 1: output += "==="; break; // Three pad chars
				case 2: output += "=="; break; // Two pad chars
				case 3: output += "="; break; // One pad char
				default: throw new System.Exception("Illegal base64url string!");
			}

			var converted = Convert.FromBase64String(output); // Standard base64 decoder
			return converted;
		}
	}
}
