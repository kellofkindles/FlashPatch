# FlashPatch!

[![Patreon](https://img.shields.io/badge/Kofi-donate-purple.svg)](https://ko-fi.com/disyer) [![MIT license](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/darktohka/FlashPatch/blob/master/LICENSE)

![Image of FlashPatch](https://i.imgur.com/OGoinaF.png)

[Download latest version](https://github.com/darktohka/FlashPatch/releases/latest)

## What's this?

FlashPatch! is a tool that modifies the Flash Player installation on your computer, making it possible to play games in the browser again.

It bypasses the January 12th, 2021 killswitch that prevents you from playing any Flash Player game or animation after January 12th.

## Compatibility

| Browser           | Plugin API       | Version    | Windows                  | Linux                    | Mac                      | 64-bit             | 32-bit                   |
| ----------------- | ---------------- | ---------- | ------------------------ | ------------------------ | ------------------------ | ------------------ | ------------------------ |
| Google Chrome     | PPAPI (Pepper)   | 32.0.0.465 | :heavy_check_mark:       | :heavy_check_mark:       | :heavy_check_mark:       | :heavy_check_mark: | :x:                      |
| Mozilla Firefox   | NPAPI (Netscape) | 32.0.0.465 | :heavy_check_mark:       | :heavy_check_mark:       | :heavy_check_mark:       | :heavy_check_mark: | :heavy_check_mark:       |
| Safari            | NPAPI (Netscape) | 32.0.0.465 | :heavy_multiplication_x: | :heavy_multiplication_x: | :heavy_check_mark:       | :heavy_check_mark: | :heavy_multiplication_x: |
| Internet Explorer | ActiveX (OCX)    | 32.0.0.445 | :heavy_check_mark:       | :heavy_multiplication_x: | :heavy_multiplication_x: | :heavy_check_mark: | :heavy_check_mark:       |

✔️ means that the plugin is officially supported.

✖️ means that the plugin is unavailable on the platform (for example, Safari plugins are unavailable on Windows).

❌ means that a specific patch does not exist for that plugin on that platform. These plugins may still be patched using the `Patch File...` button.

**A generic patch is also available that is version independent. To try this patch, use the `Patch File...` button and manually choose the Flash binary you wish to patch.**

## Flash Player

**But I want to play games using the standalone Flash Player!**

Adobe ships a current version of the Flash Player projector without the killswitch built-in [on their debug downloads page](https://adobe.com/support/flashplayer/debug_downloads.html).

**I do not have Adobe Flash Player installed, where can I get it?**

You can download Adobe Flash Player 32.0.0.465 from [this archived link](https://web.archive.org/web/20210112063313/http://fpdownload.adobe.com/get/flashplayer/pdc/32.0.0.465/install_flash_player.exe). You might need to turn back your system clock to January 11th, 2021 or earlier to install it.

## Linux

**I would like to use Flash Player on Linux!**

Unfortunately, the tool only works on Windows right now, but if you have a Windows machine, feel free to manually patch version 32.0.0.465 of `libpepflashplayer.so` or `libflashplayer.so`. You can then use these binaries on your Linux machine.

## Windows XP

**I need to use Flash Player on Windows XP!**

FlashPatch is not compatible with Windows XP. FlashPatch relies on .NET Framework 4.5, but the last supported version of .NET Framework on Windows XP is [4.0.3](https://docs.microsoft.com/en-us/dotnet/framework/install/on-windows-xp#net-framework-403). Regardless, there is a way to keep using Flash on Windows XP. [Please check out the following instructions on GitHub.](https://github.com/darktohka/FlashPatch/issues/7#issuecomment-785096536)

## Usage

- Extract `FlashPatch.exe` into a new folder.
- Run `FlashPatch.exe` and allow it to run as administrator (necessary to change system Flash Player files)
- Press the "Patch" button and agree to the warning message
- Your Flash Player is now usable once again!

To restore the old, unpatched binaries:

- The unpatched binaries are saved into a `Backup` folder when you patch Flash Player
- Run `FlashPatch.exe` and allow it to run as administrator (necessary to change system Flash Player files)
- Press the "Restore" button and agree
- Your Flash Player is now back to its factory defaults.
