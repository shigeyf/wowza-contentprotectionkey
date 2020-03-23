# Wowza Content Protection Key Command-line Tool
A command-line tool for generating content protection key file(s) for Wowza Streaming Engine.

## Overview
This command-line tool is used to create a key file for Content Protection in Wowza Streaming Engine.

Wowza Streaming Engine can support on-the-fly packaging with both AES Clear-key encryption and DRM (Digital Rights Management) Encryption. This tool will create a content key file for the on-the-fly encryption by giving the content key information for content encryption.

## Build this tool
This tool is built with .NET Core. You can follow the [instructions](https://docs.microsoft.com/en-us/dotnet/core/install/) to install .NET Core SDK & Runtime on the supported platforms, Windows, Linux, and macOS.

```command line
git clone https://github.com/shigeyf/wowza-contentprotectionkey
cd wowza-contentprotectionkey
dotnet build
```

## Run this tool

```command line
dotnet run -- <options>
```

## Command-line Tool Details

### Description

`WowzaContentProtectionKey` - Generates a content protection key file for Wowza Streaming Engine.

### Synopsis

```command line
wowzacontentprotectionkey [-k|--keyid <Key Id Hex string>] [-b|--keybase64str <Key Value Base64 string>]
    [-p|--enable-playready] [--playready-license-server-url <PlayReady License Server URL>]
    [-w|--enable-widevine] [--widevine-content-id <Widevine Content Id>]
    [--enable-mpegdashstreaming] [--disable-mpegdashstreaming]
    [--enable-cmafstreaming] [--disable-cmafstreaming]

wowzacontentprotectionkey [-?|-h|--help]
```

### Options

- **`-k|--key-id <Key Id Hex string>`**

  Specify a hex string of Content Key Identifier.

- **`-b|--keybase64str <Key Value Base64 string>`**

  Specify a Base64 encoded string of Content Key for encryption.

- **`-p|--enable-playready`**

  Enable PlayReady DRM protection.

- **`--playready-license-server-url <PlayReady License Server URL>`**

  Specify a PlayReady licenser server URL for DRM license acquisition.

- **`-w|--enable-widevine`**

  Enable Widevine DRM protection.

- **`--widevine-content-id <Widevine Content Id>`**

  Specify a Widevine Content Identifier.

- **`--enable-cmafstreaming`** OR **`--disable-cmafstreaming`**

  Enable or disable content protection to CMAF Streaming.

- **`--enable-mpegdashstreaming`** OR **`--disable-mpegdashstreaming`**

  Enable or disable content protection to MPEG-DASH Streaming.

### Example

```command line
CMD> wowzacontentprotectionkey -k 50b115acace65f7aa8d0b1c28b371c04 -b "Pm0mygqqyOBY8OvD/6rSzg==" \
    -p --playready-license-server-url "https://test.playready.microsoft.com/service/rightsmanager.asmx?cfg=(persist:false,sl:150,contentkey:PvTWCAzver0zdcLrchwK2w==)" \
    -w --widevine-content-id shigeyf-wv-singlekey \
    --enable-mpegdashstreaming --enable-cmafstreaming
```

```Output
cmafstreaming-cenc-algorithm: AESCTR
cmafstreaming-cenc-content-key: Pm0mygqqyOBY8OvD/6rSzg==
cmafstreaming-cenc-key-id: 50b115ac-ace6-5f7a-a8d0-b1c28b371c04
cmafstreaming-cenc-keyserver-playready-checksum: MwROBJRAfBA=
cmafstreaming-cenc-keyserver-playready-license-url: https://test.playready.microsoft.com/service/rightsmanager.asmx?cfg=(persist:false,sl:150,contentkey:PvTWCAzver0zdcLrchwK2w==)
cmafstreaming-cenc-keyserver-playready-system-id: 9a04f079-9840-4286-ab92-e65be0885f95
cmafstreaming-cenc-keyserver-playready: true
cmafstreaming-cenc-keyserver-widevine-pssh-data: IhRzaGlnZXlmLXd2LXNpbmdsZWtleQ==
cmafstreaming-cenc-keyserver-widevine-system-id: edef8ba9-79d6-4ace-a3c8-27dcd51d21ed
cmafstreaming-cenc-keyserver-widevine: true
mpegdashstreaming-cenc-algorithm: AESCTR
mpegdashstreaming-cenc-content-key: Pm0mygqqyOBY8OvD/6rSzg==
mpegdashstreaming-cenc-key-id: 50b115ac-ace6-5f7a-a8d0-b1c28b371c04
mpegdashstreaming-cenc-keyserver-playready-checksum: MwROBJRAfBA=
mpegdashstreaming-cenc-keyserver-playready-license-url: https://test.playready.microsoft.com/service/rightsmanager.asmx?cfg=(persist:false,sl:150,contentkey:PvTWCAzver0zdcLrchwK2w==)
mpegdashstreaming-cenc-keyserver-playready-system-id: 9a04f079-9840-4286-ab92-e65be0885f95
mpegdashstreaming-cenc-keyserver-playready: true
mpegdashstreaming-cenc-keyserver-widevine-pssh-data: IhRzaGlnZXlmLXd2LXNpbmdsZWtleQ==
mpegdashstreaming-cenc-keyserver-widevine-system-id: edef8ba9-79d6-4ace-a3c8-27dcd51d21ed
mpegdashstreaming-cenc-keyserver-widevine: true
```
