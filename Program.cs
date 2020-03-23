//
// Program.cs
//

using System;
using System.Linq;
using System.Text;
using McMaster.Extensions.CommandLineUtils;


namespace WowzaContentProtectionKey
{
    [Command(Name = "wowzacontentprotectionkey", Description = "Generate content protection key file(s) for Wowza Streaming Engine")]
    class Program
    {
        public static int Main(string[] args)
            => CommandLineApplication.Execute<Program>(args);

        [Option("-k|--keyid", Description = "Hex string of Content Key Identifier.")]
        public string HexKeyIdString { get; }

        [Option("-b|--keybase64str", Description = "Base64 string of content key bytes.")]
        public string Base64KeyBytesString { get; }

        [Option("-p|--enable-playready", Description = "Enable PlayReady DRM.")]
        public bool PlayReadyEnabled { get; }

        [Option("--playready-license-server-url", Description = "License Server URL for PlayReady DRM.")]
        public string PlayReadyLicenseServerUrl { get; }

        [Option("-w|--enable-widevine", Description = "Enable Widevine DRM.")]
        public bool WidevineEnabled { get; }

        [Option("--widevine-content-id", Description = "Content Id for Widevine DRM.")]
        public string WidevineContentId { get; }

        [Option("--enable-mpegdashstreaming", Description = "Enable content encryption to MPEG-DASH Streaming (default).")]
        public bool MpegDashStreamingEnabled { get; }

        [Option("--disable-mpegdashstreaming", Description = "Disable content encryption to MPEG-DASH Streaming.")]
        public bool MpegDashStreamingDisabled { get; }

        [Option("--enable-cmafstreaming", Description = "Enable content encryption to CMAF MPEG-DASH Streaming.")]
        public bool CmafStreamingEnabled { get; }

        [Option("--disable-cmafstreaming", Description = "Disable content encryption to MPEG-DASH Streaming.")]
        public bool CmafStreamingDisabled { get; }


        private int OnExecute(CommandLineApplication app)
        {
            if (string.IsNullOrEmpty(HexKeyIdString) || string.IsNullOrEmpty(Base64KeyBytesString))
            {
                app.ShowHelp();
                return 1;
            }

            WowzaContentProtectionKey wcpkey = new WowzaContentProtectionKey(HexKeyIdString, Base64KeyBytesString);

            if (MpegDashStreamingDisabled && MpegDashStreamingEnabled)
            {
                app.ShowHelp();
                return 1;
            }
            if (CmafStreamingDisabled && CmafStreamingEnabled)
            {
                app.ShowHelp();
                return 1;
            }
            wcpkey.SetStreamingMpegDash(MpegDashStreamingEnabled, MpegDashStreamingDisabled);
            wcpkey.SetStreamingCmaf(CmafStreamingEnabled, CmafStreamingDisabled);

            if (PlayReadyEnabled)
            {
                if (string.IsNullOrEmpty(PlayReadyLicenseServerUrl))
                {
                    app.ShowHelp();
                    return 1;
                }
                wcpkey.EnablePlayReady(PlayReadyLicenseServerUrl);
            }

            if (WidevineEnabled)
            {
                if (string.IsNullOrEmpty(WidevineContentId))
                {
                    app.ShowHelp();
                    return 1;
                }
                wcpkey.EnableWidevine(WidevineContentId);
            }

            string key = wcpkey.GenerateWowzaKey();
            Console.WriteLine(key);

            return 0;
        }
    }
}

// Sample #1
//HexKeyIdString = "30303030303030303030303030303035";
//Base64KeyBytesString = "msMDbgSsnSvpRu1iQFFJvA==";
//Widevine Provider = "widevine_test";
//Widevine ContentId = "2015_tears";

// Sample #2
//HexkeyIdString = "50b115acace65f7aa8d0b1c28b371c04";
//Base64KeyBytesString = "Pm0mygqqyOBY8OvD/6rSzg==";
//Widevine Provider = "widevine_test";
//Widevine ContentId = "shigeyf-wv-singlekey";
