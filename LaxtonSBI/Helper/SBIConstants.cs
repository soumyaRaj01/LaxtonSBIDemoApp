using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaxtonSBI.Helper
{
    public class SBIConstants
    {
        // SBI Device Type Name
        public static string FINGERPRINT = "Finger";
        public static string FACE = "Face";
        public static string IRIS = "Iris";

        // SBI Device SubType Name
        public static string DEVICE_SUBTYPE_FACE_FULLFACE = "Full face";
        public static string DEVICE_SUBTYPE_IRIS_SINGLE = "Single";
        public static string DEVICE_SUBTYPE_IRIS_DOUBLE = "Double";
        public static string DEVICE_SUBTYPE_FINGER_SLAP = "Slap";

        //Mosip Methods
        public static string MOSIP_METHOD_MOSIPDISC = "MOSIPDISC";
        public static string MOSIP_METHOD_MOSIPDINFO = "MOSIPDINFO";
        public static string MOSIP_METHOD_CAPTURE = "RCAPTURE";
        public static string MOSIP_METHOD_STREAM = "STREAM";
        public static string MOSIP_METHOD_RCAPTURE = "RCAPTURE";

        // Endpoints
        public static string BASE_URI = "http://localhost:4503";
        public static string INFO = "/info";
        public static string CAPTURE = "/capture";
        public static string STREAM = "/stream";

        // Captue Finger options
        public static string ALL_FINGERS = "Capture all fingers";
        public static string LEFT_FINGERS = "Capture left fingers";
        public static string RIGHT_FINGERS = "Capture right fingers";

        // Captue Iris options
        public static string ALL_IRISES = "Capture all irises";
        public static string LEFT_IRIS = "Capture left iris";
        public static string RIGHT_IRIS = "Capture right iris";

        
    }
}
