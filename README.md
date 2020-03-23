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
