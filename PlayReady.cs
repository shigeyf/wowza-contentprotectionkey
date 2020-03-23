//
// PlayReady.cs
//

using System;
using System.Security.Cryptography;


namespace WowzaContentProtectionKey
{
    public class PlayReady
    {
        public bool Enabled { get; set; }
        public string LicenseServerUrl { get; }
        public string KeyChecksum { get; }

        // Content Key
        private byte[] keyId;
        private string keyIdUUID;
        private byte[] keyBytes;
        private Guid keyIdGuid;
        private byte[] keySeedBytes;
        private byte[] keyChecksumBytes;

        // Static variables
        public static Guid SystemId = new Guid("9a04f079-9840-4286-ab92-e65be0885f95");
        public static string PlayReadyTestKeySeedB64 = "XVBovsmzhP9gRIZxWfFta3VVRPzVEWmJsazEJ46I";
        public static byte[] PlayReadyTestKeySeedBytes = { 0x5D, 0x50, 0x68, 0xBE, 0xC9, 0xB3, 0x84, 0xFF, 0x60, 0x44, 0x86, 0x71, 0x59, 0xF1, 0x6D, 0x6B, 0x75, 0x55, 0x44, 0xFC, 0xD5, 0x11, 0x69, 0x89, 0xB1, 0xAC, 0xC4, 0x27, 0x8E, 0x88 };

        public PlayReady(byte[] keyId, string keyIdUUID, byte[] keyBytes, string licenseServerUrl, string keySeed = null)
        {
            this.keyId = keyId;
            this.keyIdUUID = keyIdUUID;
            this.keyBytes = keyBytes;
            this.keyIdGuid = new Guid(keyIdUUID);

            this.keySeedBytes = keySeed != null ? Convert.FromBase64String(keySeed) : PlayReadyTestKeySeedBytes;
            byte[] keyIdGuidBytes = this.keyIdGuid.ToByteArray();
            if (keyBytes == null)
            {
                keyBytes = GeneratePlayReadyContentKey(this.keySeedBytes, this.keyIdGuid);
            }
            this.keyChecksumBytes = GeneratePlayReadyContentKeyAesCtrChecksum(keyIdGuidBytes, keyBytes);

            this.LicenseServerUrl = licenseServerUrl;
            this.KeyChecksum = Convert.ToBase64String(this.keyChecksumBytes);
            this.Enabled = true;
        }

        /*
         * Calculating PlayReady Content Key with Key Seed
         * https://docs.microsoft.com/en-us/playready/specifications/playready-key-seed
         * 
         * Services implementing PlayReady must maintain a Key Management System (KMS) to store and manage content keys.
         * Specifically, the values of {KID, Content Key} are stored for each content asset that is managed by the service.
         * These values are stored at encryption time, and retrieved at license issuance time.
         * 
         * PlayReady provides a convenient way to avoid a complex KMS.
         * The Content Key Seed algorithm allows derivation of different content keys for a collection of content assets,
         *  from a varying KID and a fixed Key Seed:
         *     Ck(KID) = f(KID, KeySeed)
         * 
         * !!NOTE!!:
         *   The algorithm is expected to input KeyId as GUID.
         *   This means the byte data of the KeyId must be GUID LE Byte Array.
         * 
         * The following is the PlayReady standard algorithm:
         */
        public static byte[] GeneratePlayReadyContentKey(byte[] keySeed, Guid keyId)
        {
            const int DRM_AES_KEYSIZE_128 = 16;
            byte[] contentKey = new byte[DRM_AES_KEYSIZE_128];
            //
            //  Truncate the key seed to 30 bytes, key seed must be at least 30 bytes long.
            //
            byte[] truncatedKeySeed = new byte[30];
            Array.Copy(keySeed, truncatedKeySeed, truncatedKeySeed.Length);
            //
            //  Get the keyId as a byte array
            //
            byte[] keyIdAsBytes = keyId.ToByteArray();
            //
            //  Create sha_A_Output buffer.  It is the SHA of the truncatedKeySeed and the keyIdAsBytes
            //
            SHA256Managed sha_A = new SHA256Managed();
            sha_A.TransformBlock(truncatedKeySeed, 0, truncatedKeySeed.Length, truncatedKeySeed, 0);
            sha_A.TransformFinalBlock(keyIdAsBytes, 0, keyIdAsBytes.Length);
            byte[] sha_A_Output = sha_A.Hash;
            //
            //  Create sha_B_Output buffer.  It is the SHA of the truncatedKeySeed, the keyIdAsBytes, and
            //  the truncatedKeySeed again.
            //
            SHA256Managed sha_B = new SHA256Managed();
            sha_B.TransformBlock(truncatedKeySeed, 0, truncatedKeySeed.Length, truncatedKeySeed, 0);
            sha_B.TransformBlock(keyIdAsBytes, 0, keyIdAsBytes.Length, keyIdAsBytes, 0);
            sha_B.TransformFinalBlock(truncatedKeySeed, 0, truncatedKeySeed.Length);
            byte[] sha_B_Output = sha_B.Hash;
            //
            //  Create sha_C_Output buffer.  It is the SHA of the truncatedKeySeed, the keyIdAsBytes,
            //  the truncatedKeySeed again, and the keyIdAsBytes again.
            //
            SHA256Managed sha_C = new SHA256Managed();
            sha_C.TransformBlock(truncatedKeySeed, 0, truncatedKeySeed.Length, truncatedKeySeed, 0);
            sha_C.TransformBlock(keyIdAsBytes, 0, keyIdAsBytes.Length, keyIdAsBytes, 0);
            sha_C.TransformBlock(truncatedKeySeed, 0, truncatedKeySeed.Length, truncatedKeySeed, 0);
            sha_C.TransformFinalBlock(keyIdAsBytes, 0, keyIdAsBytes.Length);
            byte[] sha_C_Output = sha_C.Hash;
            for (int i = 0; i < DRM_AES_KEYSIZE_128; i++)
            {
                contentKey[i] = Convert.ToByte(sha_A_Output[i] ^ sha_A_Output[i + DRM_AES_KEYSIZE_128]
                                               ^ sha_B_Output[i] ^ sha_B_Output[i + DRM_AES_KEYSIZE_128]
                                               ^ sha_C_Output[i] ^ sha_C_Output[i + DRM_AES_KEYSIZE_128]);
            }

            return contentKey;
        }

        /*
         * Key Checksum Algorithm
         * https://docs.microsoft.com/en-us/playready/specifications/playready-header-specification#5-key-checksum-algorithm
         * 
         * The checksum algorithm in the PlayReady Header is intended to protect against mismatched keys.
         * In the early days of DRM, songs were encrypted with incorrectly labeled keys.
         * This resulted in white noise being played back when the songs were decrypted.
         * And if the songs were played back loud enough, the playback equipment was destroyed.
         * With the checksum, the content key can be verified as the key that was used to encrypt the file.
         * 
         * !!NOTE!!:
         *   The algorithm is expected to input KeyId as GUID.
         *   This means the byte data of the KeyId must be GUID LE Byte Array.
         *   
         * The algorithm works as follows:
         * 
         * For an ALGID value set to “AESCBC”:
         *  there is no Key Checksum Algorithm defined. The CHECKSUM attribute must be omitted.
         * 
         * For an ALGID value set to “AESCTR”,
         *  the 16-byte Key ID is encrypted with a 16-byte AES content key using ECB mode.
         *  The first 8 bytes of the buffer is extracted and base64 encoded.
         * 
         * For an ALGID value set to “COCKTAIL”, perform the following steps:
         *   1. A 21-byte buffer is created.
         *   2. The content key is put in the buffer and the rest of the buffer is filled with zeros.
         *   3. For five iterations:
         *       a. buffer = SHA-1 (buffer).
         *   4. The first 7 bytes of the buffer are extracted and base64 encoded.
         *   5. After these steps are performed, the base64-encoded bytes are used as the checksum.
         * 
         */
        public static byte[] GeneratePlayReadyContentKeyAesCtrChecksum(byte[] keyId, byte[] keyBytes)
        {
            AesManaged aesEncrypt = new AesManaged();
            aesEncrypt.KeySize = 128;
            aesEncrypt.Key = keyBytes;
            aesEncrypt.BlockSize = 128;
            aesEncrypt.Mode = CipherMode.ECB;
            aesEncrypt.Padding = PaddingMode.Zeros;
            aesEncrypt.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            ICryptoTransform encryptor = aesEncrypt.CreateEncryptor(aesEncrypt.Key, aesEncrypt.IV);
            byte[] encrypted = encryptor.TransformFinalBlock(keyId, 0, keyId.Length);
            byte[] checksumBytes = new byte[8];
            Buffer.BlockCopy(encrypted, 0, checksumBytes, 0, 8);
            return checksumBytes;
        }
    }
}
