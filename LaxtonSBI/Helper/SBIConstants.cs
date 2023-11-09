using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaxtonSBI.Helper
{
    public class SBIConstants
    {
        public static string TimeStampFormat = "yyyy-MM-ddTHH:mm:ssZ";

        //Environment
        public static string ENVIRONMENT_NONE = "None";
        public static string ENVIRONMENT_STAGING = "Staging";
        public static string ENVIRONMENT_DEVELOPER = "Developer";
        public static string ENVIRONMENT_PRE_PRODUCTION = "Pre-Production";
        public static string ENVIRONMENT_PRODUCTION = "Production";

        //Purpose
        public static string PURPOSE_AUTH = "Auth";
        public static string PURPOSE_REGISTRATION = "Registration";

        // SBI Device Type Name
        public const string FINGERPRINT = "Finger";
        public const string FACE = "Face";
        public const string IRIS = "Iris";

        //Device Status
        public static string DEVICE_STATUS = "DEVICE_STATUS";
        public static string DEVICE_STATUS_ISREADY = "Ready";
        public static string DEVICE_STATUS_ISBUSY = "Busy";
        public static string DEVICE_STATUS_NOTREADY = "Not Ready";
        public static string DEVICE_STATUS_NOTREGISTERED = "Not Registered";

        //Device SubType Id Value
        public const string DEVICE_IRIS_SUB_TYPE_ID_LEFT = "1"; // LEFT IRIS IMAGE
        public const string DEVICE_IRIS_SUB_TYPE_ID_RIGHT = "2";    // RIGHT IRIS IMAGE
        public const string DEVICE_IRIS_SUB_TYPE_ID_BOTH = "3"; // BOTH LEFT AND RIGHT IRIS IMAGE
        public const string DEVICE_FINGER_SLAP_SUB_TYPE_ID_LEFT = "1";  // LEFT SLAP IMAGE
        public const string DEVICE_FINGER_SLAP_SUB_TYPE_ID_RIGHT = "2"; // RIGHT SLAP IMAGE
        public const string DEVICE_FINGER_SLAP_SUB_TYPE_ID_THUMB = "3";// TWO THUMB IMAGE
        public const string DEVICE_FACE_SUB_TYPE_ID_FULLFACE = "0";    // TWO THUMB IMAGE

        //Bio Exceptions/Bio Subtype Names
        public const string BIO_NAME_UNKNOWN = "UNKNOWN";
        public const string BIO_NAME_RIGHT_THUMB = "Right Thumb";
        public const string BIO_NAME_RIGHT_INDEX = "Right IndexFinger";
        public const string BIO_NAME_RIGHT_MIDDLE = "Right MiddleFinger";
        public const string BIO_NAME_RIGHT_RING = "Right RingFinger";
        public const string BIO_NAME_RIGHT_LITTLE = "Right LittleFinger";
        public const string BIO_NAME_LEFT_THUMB = "Left Thumb";
        public const string BIO_NAME_LEFT_INDEX = "Left IndexFinger";
        public const string BIO_NAME_LEFT_MIDDLE = "Left MiddleFinger";
        public const string BIO_NAME_LEFT_RING = "Left RingFinger";
        public const string BIO_NAME_LEFT_LITTLE = "Left LittleFinger";
        public const string BIO_NAME_RIGHT_IRIS = "Right";
        public const string BIO_NAME_LEFT_IRIS = "Left";

        // Bio SubTypes arrays
        public static List<string> BIO_NAME_LEFT_FINGERS = new List<string> { BIO_NAME_LEFT_LITTLE, BIO_NAME_LEFT_RING, BIO_NAME_LEFT_MIDDLE, BIO_NAME_LEFT_INDEX };
        public static List<string> BIO_NAME_RIGHT_FINGERS = new List<string> { BIO_NAME_RIGHT_INDEX, BIO_NAME_RIGHT_MIDDLE, BIO_NAME_RIGHT_RING, BIO_NAME_RIGHT_LITTLE };
        public static List<string> BIO_NAME_THUMB = new List<string> { BIO_NAME_LEFT_THUMB, BIO_NAME_RIGHT_THUMB };

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

        // Capture Face options
        public const string FACE_OPTION = "Capture face";

        // Captue Finger options
        public const string THUMBS_OPTION = "Capture thumbs";
        public const string LEFT_FINGERS_OPTION = "Capture left fingers";
        public const string RIGHT_FINGERS_OPTION = "Capture right fingers";

        // Captue Iris options
        public const string ALL_IRISES_OPTION = "Capture all irises";
        public const string LEFT_IRIS_OPTION = "Capture left iris";
        public const string RIGHT_IRIS_OPTION = "Capture right iris";


     
    


    }
}
