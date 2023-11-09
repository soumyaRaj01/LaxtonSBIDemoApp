using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeImageAPI;

namespace LaxtonSBI.Helper
{
    public class ImageHelper
    {
        public static string fileName { get; set; }
        public static string JPGImageFileName { get; set; }
        public static string ISOImageFileName { get; set; }
        public static string BatchJobPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory + @"bioutils";
        public static string ImagePath { get; set; } = AppDomain.CurrentDomain.BaseDirectory + @"bioutils\BiometricInfo\";

        public static byte[] ISOtoBytes(byte[] img, string type)
        {
            //fileName = "info_" + bioSubType;
            //JPGImageFileName = fileName + ".iso.jpg";
            //ISOImageFileName = fileName + ".iso";

            JPGImageFileName = "info.iso.jpg";
            ISOImageFileName = "info.iso";

            DirectoryInfo di = new DirectoryInfo(ImagePath + type);

            if (di.Exists)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            else
            {
                di.Create();
            }
            File.WriteAllBytes(ImagePath + type + "\\" + ISOImageFileName, img);

            //TODO convert ISO to JP2 Image using the JAVA utility
            var JP2Image = ISOtoJP2(ImagePath + type, JPGImageFileName, type);
            return JP2Image;
        }

        public static byte[] ISOtoJP2(string ISOPath, string JP2ImageFileName, string type)
        {
            string batchjobCommand = GetBatchJobCommand(type);
            int exitCode;

            ProcessStartInfo pInfo = new ProcessStartInfo("cmd.exe", "/c " + batchjobCommand)
            {
                WorkingDirectory = BatchJobPath,
                CreateNoWindow = false,
                UseShellExecute = true,
                LoadUserProfile = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process p = Process.Start(pInfo);
            p.WaitForExit();
            exitCode = p.ExitCode;
            p.Close();

            var JP2ImageBytes = File.ReadAllBytes(ISOPath + @"\" + JP2ImageFileName);

            return JP2ImageBytes;
        }

        private static string GetBatchJobCommand(string type)
        {
            string imageType = "io.mosip.biometrics.util.image.type.jp2000=0";
            string converTo = "io.mosip.biometrics.util.convert.iso.to.image=1";
            string converionFile = "mosip.mock.sbi.biometric.type.file.image=" + ISOImageFileName;
            string biometricSubType = "mosip.mock.sbi.biometric.subtype.unknown=UNKNOWN";
            string purpose = "io.mosip.biometrics.util.purpose.registration=REGISTRATION";

            string biometricFaceFolderPath = "mosip.mock.sbi.biometric.type.face.folder.path=/BiometricInfo/Face/";
            string biometricFingerFolderPath = "mosip.mock.sbi.biometric.type.finger.folder.path=/BiometricInfo/Finger/";
            string biometricIrisFolderPath = "mosip.mock.sbi.biometric.type.iris.folder.path=/BiometricInfo/Iris/";

            string biometricFolderPath = "";

            switch (type)
            {
                case SBIConstants.FACE:
                    biometricFolderPath = biometricFaceFolderPath;
                    break;
                case SBIConstants.FINGERPRINT:
                    biometricFolderPath = biometricFingerFolderPath;
                    break;
                case SBIConstants.IRIS:
                    biometricFolderPath = biometricIrisFolderPath;
                    break;
            }

            string batchjobCommand =
            string.Format(@"java -cp bioutils-0.0.1-SNAPSHOT.jar;lib\* io.mosip.biometrics.util.test.BioUtilApplication {0} {1} {2} {3} {4} {5}"
                , imageType
                , converTo
                , biometricFolderPath
                , converionFile
                , biometricSubType
                , purpose);

            return batchjobCommand;
        }
    }
}
