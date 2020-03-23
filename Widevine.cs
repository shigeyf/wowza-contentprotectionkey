//
// Widevine.cs
//

using System;
using System.Text;
using Google.Protobuf;


namespace WowzaContentProtectionKey
{
    public class Widevine
    {
        public bool Enabled { get; set; }
        public string Provider { get; set; }
        public string ContentId { get; set; }
        public string PsshData { get; set; }

        // Content Key
        private byte[] keyId;
        private string keyIdUUID;
        private byte[] keyBytes;

        // Static variables
        public static Guid SystemId = new Guid("edef8ba9-79d6-4ace-a3c8-27dcd51d21ed");

        public Widevine(byte[] keyId, string keyIdUUID, byte[] keyBytes, string contentId)
        {
            this.keyId = keyId;
            this.keyIdUUID = keyIdUUID;
            this.keyBytes = keyBytes;
            this.ContentId = contentId;

            if (this.ContentId != null)
            {
                ByteString widevineKeyId = ByteString.CopyFrom(this.keyId);
                ByteString widevineContentId = ByteString.CopyFrom(Encoding.ASCII.GetBytes(this.ContentId));

                WidevinePsshData widevinePsshData = new WidevinePsshData
                {
                    //Algorithm = WidevinePsshData.Types.Algorithm.Aesctr,
                    //KeyId = { widevineKeyId },
                    //Provider = this.Provider,
                    ContentId = widevineContentId
                };
                this.PsshData = Convert.ToBase64String(widevinePsshData.ToByteArray());
            }

            this.Enabled = true;
        }
    }
}
