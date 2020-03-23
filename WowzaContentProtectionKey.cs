//
// WowzaKey.cs
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WowzaContentProtectionKey
{
    public class WowzaContentProtectionKey
    {
        #region static variables

        public static string CencAlgorithmAesCtr = "AESCTR";

        #region mpegdashstreaming directives
        public static string MpegDashCencKeyId = "mpegdashstreaming-cenc-key-id";
        public static string MpegDashCencContentKey = "mpegdashstreaming-cenc-content-key";
        public static string MpegDashCencAlgorithm = "mpegdashstreaming-cenc-algorithm";
        public static string MpegDashCencKeyServerPlayReady = "mpegdashstreaming-cenc-keyserver-playready";
        public static string MpegDashCencKeyServerPlayReadySystemId = "mpegdashstreaming-cenc-keyserver-playready-system-id";
        public static string MpegDashCencKeyServerPlayReadyLicenseUrl = "mpegdashstreaming-cenc-keyserver-playready-license-url";
        public static string MpegDashCencKeyServerPlayReadyChecksum = "mpegdashstreaming-cenc-keyserver-playready-checksum";
        public static string MpegDashCencKeyServerWidevine = "mpegdashstreaming-cenc-keyserver-widevine";
        public static string MpegDashCencKeyServerWidevineSystemId = "mpegdashstreaming-cenc-keyserver-widevine-system-id";
        public static string MpegDashCencKeyServerWidevinePsshData = "mpegdashstreaming-cenc-keyserver-widevine-pssh-data";
        #endregion

        #region cmafstreaming directives
        public static string CmafCencKeyId = "cmafstreaming-cenc-key-id";
        public static string CmafCencContentKey = "cmafstreaming-cenc-content-key";
        public static string CmafCencAlgorithm = "cmafstreaming-cenc-algorithm";
        public static string CmafCencKeyServerPlayReady = "cmafstreaming-cenc-keyserver-playready";
        public static string CmafCencKeyServerPlayReadySystemId = "cmafstreaming-cenc-keyserver-playready-system-id";
        public static string CmafCencKeyServerPlayReadyLicenseUrl = "cmafstreaming-cenc-keyserver-playready-license-url";
        public static string CmafCencKeyServerPlayReadyChecksum = "cmafstreaming-cenc-keyserver-playready-checksum";
        public static string CmafCencKeyServerWidevine = "cmafstreaming-cenc-keyserver-widevine";
        public static string CmafCencKeyServerWidevineSystemId = "cmafstreaming-cenc-keyserver-widevine-system-id";
        public static string CmafCencKeyServerWidevinePsshData = "cmafstreaming-cenc-keyserver-widevine-pssh-data";
        #endregion

        #endregion

        public StringBuilder output = new StringBuilder();
        List<string> keys = new List<string>();
        public byte[] keyId;
        public string keyIdUUID;
        public byte[] keyBytes;
        public StreamingProto streaming = new StreamingProto();
        protected PlayReady playready = null;
        protected Widevine widevine = null;

        public class StreamingProto
        {
            public bool enableMpegDash = false;
            public bool enableCmaf = false;
        }

        public WowzaContentProtectionKey(string keyIdString, string keyBytesBase64)
        {
            this.keyId = StringToByteArray(keyIdString);
            this.keyIdUUID = ByteArrayToUuidString(keyId);
            this.keyBytes = Convert.FromBase64String(keyBytesBase64);
            this.streaming.enableMpegDash = true;
        }

        public void EnablePlayReady(string licenseServerUrl, string keySeed = null)
        {
            this.playready = new PlayReady(this.keyId, this.keyIdUUID, this.keyBytes, licenseServerUrl, keySeed);
        }
        public void EnableWidevine(string contentId)
        {
            this.widevine = new Widevine(this.keyId, this.keyIdUUID, this.keyBytes, contentId);
        }
        public void SetStreamingMpegDash(bool enabled, bool disabled)
        {
            if (enabled) this.streaming.enableMpegDash = true;
            if (disabled) this.streaming.enableMpegDash = false;
        }
        public void SetStreamingCmaf(bool enabled, bool disabled)
        {
            if (enabled) this.streaming.enableCmaf = true;
            if (disabled) this.streaming.enableCmaf = false;
        }

        public string GenerateWowzaKey()
        {
            byte[] keyId = this.keyId;
            string keyIdUUID = this.keyIdUUID;

            //
            // Basic Content Key Information
            //
            if (streaming.enableMpegDash)
            {
                Add(MpegDashCencKeyId, keyIdUUID);
                Add(MpegDashCencContentKey, Convert.ToBase64String(keyBytes));
                Add(MpegDashCencAlgorithm, CencAlgorithmAesCtr);
            }
            if (streaming.enableCmaf)
            {
                Add(CmafCencKeyId, keyIdUUID);
                Add(CmafCencContentKey, Convert.ToBase64String(keyBytes));
                Add(CmafCencAlgorithm, CencAlgorithmAesCtr);
            }

            //
            // PlayReady DRM Content Key Information
            //
            if (this.playready != null && this.playready.Enabled)
            {
                if (streaming.enableMpegDash)
                {
                    Add(MpegDashCencKeyServerPlayReady, this.playready.Enabled.ToString().ToLower());
                    Add(MpegDashCencKeyServerPlayReadySystemId, PlayReady.SystemId.ToString());
                    Add(MpegDashCencKeyServerPlayReadyLicenseUrl, this.playready.LicenseServerUrl);
                    Add(MpegDashCencKeyServerPlayReadyChecksum, this.playready.KeyChecksum);
                }
                if (streaming.enableCmaf)
                {
                    Add(CmafCencKeyServerPlayReady, this.playready.Enabled.ToString().ToLower());
                    Add(CmafCencKeyServerPlayReadySystemId, PlayReady.SystemId.ToString());
                    Add(CmafCencKeyServerPlayReadyLicenseUrl, this.playready.LicenseServerUrl);
                    Add(CmafCencKeyServerPlayReadyChecksum, this.playready.KeyChecksum);
                }
            }

            //
            // Widevine DRM Content Key Information
            //
            if (this.widevine != null && this.widevine.Enabled)
            {
                if (streaming.enableMpegDash)
                {
                    Add(MpegDashCencKeyServerWidevine, this.widevine.Enabled.ToString().ToLower());
                    Add(MpegDashCencKeyServerWidevineSystemId, Widevine.SystemId.ToString());
                    Add(MpegDashCencKeyServerWidevinePsshData, this.widevine.PsshData);
                }
                if (streaming.enableCmaf)
                {
                    Add(CmafCencKeyServerWidevine, this.widevine.Enabled.ToString().ToLower());
                    Add(CmafCencKeyServerWidevineSystemId, Widevine.SystemId.ToString());
                    Add(CmafCencKeyServerWidevinePsshData, this.widevine.PsshData);
                }
            }

            Final();
            return output.ToString();
        }

        #region static methods

        private void Add(string key, string value)
        {
            string line = key + ": " + value + "\n";
            this.keys.Add(line);
        }

        private void Final()
        {
            this.keys.Sort();
            foreach (string key in this.keys)
            {
                output.Append(key);
            }
        }

        public static byte[] StringToByteArray(string hexString)
        {
            return Enumerable.Range(0, hexString.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ByteArrayToString(byte[] bytes)
        {
            StringBuilder hexString = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hexString.AppendFormat("{0:x2}", b);
            return hexString.ToString();
        }

        public static string ByteArrayToUuidString(byte[] bytes)
        {
            StringBuilder hexString = new StringBuilder(bytes.Length * 2);
            int i = 0;
            foreach (byte b in bytes)
            {
                if (i == 4) hexString.AppendFormat("-");
                if (i == 6) hexString.AppendFormat("-");
                if (i == 8) hexString.AppendFormat("-");
                if (i == 10) hexString.AppendFormat("-");
                hexString.AppendFormat("{0:x2}", b);
                i++;
            }
            return hexString.ToString();
        }
        #endregion

    }
}
