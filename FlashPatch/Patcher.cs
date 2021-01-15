﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace FlashPatch {
    public class Patcher {
        // All of these patches are for the latest Adobe Flash
        // Player versions released until January 12, 2021
        private static List<PatchableBinary> binaries = new List<PatchableBinary>() {
            new PatchableBinary(
                "Chrome 64-bit Plugin (Pepper)", "pepflashplayer64_32_0_0_465.dll", "32,0,0,465", true, 32002616, new List<HexPatch>() {
                new HexPatch(
                    0x2675F0,
                    new byte[] { 0x48, 0x89, 0x6C, 0x24, 0x18, 0x48, 0x89, 0x74, 0x24, 0x20 },
                    new byte[] { 0xB8, 0x00, 0x00, 0x00, 0x00, 0xC3, 0x90, 0x90, 0x90, 0x90 }
                ),
                new HexPatch(
                    0x2676F0,
                    new byte[] { 0x48, 0x89, 0x5C, 0x24, 0x18, 0x48, 0x89, 0x6C, 0x24, 0x20 },
                    new byte[] { 0xB8, 0x00, 0x00, 0x00, 0x00, 0xC3, 0x90, 0x90, 0x90, 0x90 }
                )},
                new List<string>() {
                    Path.Combine(GetLocalAppdata(), "Google", "Chrome", "User Data", "PepperFlash", "32.0.0.465", "pepflashplayer.dll"),
                    Path.Combine(GetLocalAppdata(), "Microsoft", "Edge", "User Data", "PepperFlash", "32.0.0.465", "pepflashplayer.dll")
                }),
            new PatchableBinary(
                "Firefox 64-bit Plugin (NPAPI)", "NPSWF64_32_0_0_465.dll", "32,0,0,465", true, 26911800, new List<HexPatch>() {
                new HexPatch(
                    0x378550,
                    new byte[] { 0x84, 0xC0, 0x74 },
                    new byte[] { 0x90, 0x90, 0xEB }
                ),
                new HexPatch(
                    0x3930B2,
                    new byte[] { 0x84, 0xC0, 0x74 },
                    new byte[] { 0x90, 0x90, 0xEB }
                ),
                new HexPatch(
                    0x39E340,
                    new byte[] { 0x84, 0xC0, 0x74 },
                    new byte[] { 0x90, 0x90, 0xEB }
                ),
                new HexPatch(
                    0x4E397B,
                    new byte[] { 0x84, 0xC0, 0x74 },
                    new byte[] { 0x90, 0x90, 0xEB }
                )
            }),
            new PatchableBinary(
                "IE 64-bit Plugin (ActiveX)", "Flash.ocx", "32,0,0,445", true, 28979096, new List<HexPatch>() {
                new HexPatch(
                    0x2A666C,
                    new byte[] { 0x84, 0xC0, 0x74 },
                    new byte[] { 0x90, 0x90, 0xEB }
                ),
                new HexPatch(
                    0x2C32FB,
                    new byte[] { 0x84, 0xC0, 0x74 },
                    new byte[] { 0x90, 0x90, 0xEB }
                ),
                new HexPatch(
                    0x2CF7DC,
                    new byte[] { 0x84, 0xC0, 0x74 },
                    new byte[] { 0x90, 0x90, 0xEB }
                )
            }),
            new PatchableBinary(
                "Firefox 32-bit Plugin (NPAPI)", "NPSWF32_32_0_0_465.dll", "32,0,0,465", false, 20404792, new List<HexPatch>() {
                new HexPatch(
                    0x2D29C5,
                    new byte[] { 0x84, 0xC0, 0x74 },
                    new byte[] { 0x90, 0x90, 0xEB }
                ),
                new HexPatch(
                    0x2E8F8A,
                    new byte[] { 0x84, 0xC0, 0x74 },
                    new byte[] { 0x90, 0x90, 0xEB }
                ),
                new HexPatch(
                    0x2F2771,
                    new byte[] { 0x84, 0xC0, 0x74 },
                    new byte[] { 0x90, 0x90, 0xEB }
                ),
                new HexPatch(
                    0x3EE74D,
                    new byte[] { 0x84, 0xC0, 0x74 },
                    new byte[] { 0x90, 0x90, 0xEB }
                )
            }),
            new PatchableBinary(
                "IE 32-bit Plugin (ActiveX)", "Flash.ocx", "32,0,0,445", false, 22874520, new List<HexPatch>() {
                new HexPatch(
                    0x2A0C9B,
                    new byte[] { 0x84, 0xC0, 0x74 },
                    new byte[] { 0x90, 0x90, 0xEB }
                ),
                new HexPatch(
                    0x2BA490,
                    new byte[] { 0x84, 0xC0, 0x74 },
                    new byte[] { 0x90, 0x90, 0xEB }
                ),
                new HexPatch(
                    0x2C53CF,
                    new byte[] { 0x84, 0xC0, 0x74 },
                    new byte[] { 0x90, 0x90, 0xEB }
                )
            })
        };

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);

        private static string GetWindowsDir() {
            return Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        }

        private static string GetLocalAppdata() {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        private static string GetFlashDir32() {
            if (Environment.Is64BitOperatingSystem) {
                return Path.Combine(GetWindowsDir(), "SysWOW64", "Macromed", "Flash");
            } else {
                return Path.Combine(GetWindowsDir(), "System32", "Macromed", "Flash");
            }
        }

        private static string GetFlashDir64() {
            if (Environment.Is64BitOperatingSystem) {
                return Path.Combine(GetWindowsDir(), "System32", "Macromed", "Flash");
            } else {
                return null;
            }
        }

        private static string GetVersion(string filename) {
            return FileVersionInfo.GetVersionInfo(filename).FileVersion;
        }

        private static bool IsSharingViolation(Exception e) {
            return ((uint)e.HResult) == 0x80070020;
        }

        private static void ShowError(string message) {
            MessageBox.Show(message, "FlashPatch!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void AppendItems(StringBuilder builder, string message, List<string> items) {
            if (items.Count <= 0) {
                return;
            }

            builder.AppendLine(message);

            foreach (string item in items) {
                builder.AppendLine(item);
            }

            builder.AppendLine();
        }

        private static void TakeOwnership(string filename) {
            FileSecurity security = new FileSecurity();

            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;
            security.SetOwner(sid);
            security.SetAccessRule(new FileSystemAccessRule(sid, FileSystemRights.FullControl, AccessControlType.Allow));

            File.SetAccessControl(filename, security);
        }

        public static void PatchAll() {
            if (MessageBox.Show("Are you sure you want to patch your system-wide Flash plugins to remove the January 12nd, 2021 killswitch and allow Flash games to be played in your browser?", "FlashPatch!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) {
                return;
            }

            if (MessageBox.Show("Have you closed ALL your browser windows yet?\n\nIf not, please close them right now!", "FlashPatch!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) {
                return;
            }

            if (MessageBox.Show("WARNING!\n\nThe developers of this program do not assume any responsibility for the usage of this tool.\n\nEven though the developers have tried their best to ensure the quality of this tool, it may introduce instability, or even crash your computer.\n\nAll changes made by the program may be reverted using the \"Restore\" button, but even this option is provided on a best-effort basis.\n\nAll responsibility falls upon your shoulders.\n\nEven so, are you sure you want to continue?", "FlashPatch!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) {
                return;
            }


            WinAPI.ModifyPrivilege(PrivilegeName.SeRestorePrivilege, true);
            WinAPI.ModifyPrivilege(PrivilegeName.SeTakeOwnershipPrivilege, true);

            IntPtr wow64Value = IntPtr.Zero;

            // Disable file system indirection (otherwise we can't read System32)
            Wow64DisableWow64FsRedirection(ref wow64Value);

            string flashDir32 = GetFlashDir32();

            if (!Directory.Exists(flashDir32)) {
                ShowError(string.Format("Could not find 32-bit Flash directory!\n\n{0} does not exist.", flashDir32));
                return;
            }

            bool x64 = Environment.Is64BitOperatingSystem;
            string flashDir64 = null;

            if (x64) {
                flashDir64 = GetFlashDir64();

                if (!Directory.Exists(flashDir64)) {
                    ShowError(string.Format("Could not find 64-bit Flash directory!\n\n{0} does not exist.", flashDir64));
                    return;
                }
            }

            List<string> patched = new List<string>();
            List<string> alreadyPatched = new List<string>();
            List<string> notFound = new List<string>();
            List<string> incompatibleVersion = new List<string>();
            List<string> incompatibleSize = new List<string>();
            List<string> ownershipFailed = new List<string>();
            List<string> locked = new List<string>();
            List<string> errors = new List<string>();

            string backupFolder = Path.Combine(Environment.CurrentDirectory, "Backup");
            bool madeBackupFolder = false;

            foreach (PatchableBinary binary in binaries) {
                bool binaryX64 = binary.IsX64();

                if (binaryX64 && !x64) {
                    // This is a 64-bit binary, but we are not on a 64-bit system.
                    continue;
                }

                string name = binary.GetName();
                List<string> paths = new List<string>();

                paths.Add(Path.Combine(binaryX64 ? flashDir64 : flashDir32, binary.GetFileName()));
                paths.AddRange(binary.GetAlternatePaths());

                bool found = false;

                foreach (string path in paths) {
                    if (!File.Exists(path)) {
                        continue;
                    }

                    found = true;
                    string version = GetVersion(path);

                    if (!binary.GetVersion().Equals(version)) {
                        // We've encountered an incompatible version.
                        incompatibleVersion.Add(string.Format("{0} ({1})", name, version));
                        continue;
                    }

                    long size = new FileInfo(path).Length;

                    if (binary.GetFileSize() != size) {
                        // This file's size does not match the expected file size.
                        incompatibleSize.Add(name);
                        continue;
                    }

                    try {
                        TakeOwnership(path);
                    } catch {
                        // We failed to get ownership of the file...
                        // No continue here, we still want to try to patch the file
                        ownershipFailed.Add(name);
                    }

                    try {
                        using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read)) {
                            if (!binary.IsPatchable(fileStream)) {
                                // This binary has already been patched.
                                alreadyPatched.Add(name);
                                continue;
                            }
                        }

                        if (!madeBackupFolder && !Directory.Exists(backupFolder)) {
                            Directory.CreateDirectory(backupFolder);
                            madeBackupFolder = true;
                        }

                        // Back up the current plugin to our backup folder
                        File.Copy(path, Path.Combine(backupFolder, binary.GetBackupFileName()), true);

                        using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite)) {
                            // Apply all pending binary patches!
                            binary.PatchFile(fileStream);
                        }

                        patched.Add(name);
                    } catch (Exception e) {
                        if (IsSharingViolation(e)) {
                            // This is a sharing violation; i.e. the file is currently being used.
                            locked.Add(name);
                        } else {
                            errors.Add(e.Message);
                        }
                    }
                }

                if (!found) {
                    notFound.Add(name);
                }
            }

            // Enable file system indirection.
            Wow64RevertWow64FsRedirection(wow64Value);

            StringBuilder report = new StringBuilder();
            MessageBoxIcon icon = MessageBoxIcon.Information;

            AppendItems(report, "Successfully patched these plugins:", patched);
            AppendItems(report, "These plugins have already been patched:", alreadyPatched);
            AppendItems(report, "These plugins have not been found on your system:", notFound);
            AppendItems(report, "These plugins are incompatible with the patch because their version is outdated:", incompatibleVersion);
            AppendItems(report, "These plugins are incompatible with the patch because their file size does not match:", incompatibleSize);
            AppendItems(report, "These plugins could not be patched because their respective browser is currently open:", locked);
            AppendItems(report, "Caught exceptions:", errors);

            if (incompatibleVersion.Count > 0 || incompatibleSize.Count > 0 || locked.Count > 0 || errors.Count > 0) {
                icon = MessageBoxIcon.Warning;
                report.AppendLine("Errors have been encountered during the patching process.\nPlease try again after reading the message above carefully.\nIf the browser you're using has been patched successfully, then no more action is required.");
            } else if (patched.Count > 0) {
                report.AppendLine("CONGRATULATIONS! The patching process has completed as expected. Enjoy your Flash games!");
            } else if (alreadyPatched.Count > 0) {
                report.AppendLine("Flash Player has already been patched on this system.\n\nNo more action is required! Enjoy your games!");
            } else {
                report.AppendLine("No action has been taken.");
            }

            MessageBox.Show(report.ToString(), "FlashPatch!", MessageBoxButtons.OK, icon);
        }


        public static void RestoreAll() {
            if (MessageBox.Show("Are you sure you want to restore your Flash Plugin backups?", "FlashPatch!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) {
                return;
            }

            string backupFolder = Path.Combine(Environment.CurrentDirectory, "Backup");

            if (!Directory.Exists(backupFolder)) {
                ShowError("No backups are currently available.");
                return;
            }

            WinAPI.ModifyPrivilege(PrivilegeName.SeRestorePrivilege, true);
            WinAPI.ModifyPrivilege(PrivilegeName.SeTakeOwnershipPrivilege, true);

            IntPtr wow64Value = IntPtr.Zero;

            // Disable file system indirection (otherwise we can't read System32)
            Wow64DisableWow64FsRedirection(ref wow64Value);

            string flashDir32 = GetFlashDir32();

            if (!Directory.Exists(flashDir32)) {
                ShowError(string.Format("Could not find 32-bit Flash directory!\n\n{0} does not exist.", flashDir32));
                return;
            }

            bool x64 = Environment.Is64BitOperatingSystem;
            string flashDir64 = null;

            if (x64) {
                flashDir64 = GetFlashDir64();

                if (!Directory.Exists(flashDir64)) {
                    ShowError(string.Format("Could not find 64-bit Flash directory!\n\n{0} does not exist.", flashDir64));
                    return;
                }
            }

            List<string> restored = new List<string>();
            List<string> locked = new List<string>();
            List<string> errors = new List<string>();

            foreach (PatchableBinary binary in binaries) {
                bool binaryX64 = binary.IsX64();

                if (binaryX64 && !x64) {
                    // This is a 64-bit binary, but we are not on a 64-bit system.
                    continue;
                }

                string backupPath = Path.Combine(backupFolder, binary.GetBackupFileName());

                if (!File.Exists(backupPath)) {
                    continue;
                }

                string name = binary.GetName();
                string path = Path.Combine(binaryX64 ? flashDir64 : flashDir32, binary.GetFileName());

                try {
                    File.Copy(backupPath, path, true);
                    restored.Add(name);
                } catch (Exception e) {
                    if (IsSharingViolation(e)) {
                        // This is a sharing violation; i.e. the file is currently being used.
                        locked.Add(name);
                    } else {
                        errors.Add(e.Message);
                    }
                }
            }

            // Enable file system indirection.
            Wow64RevertWow64FsRedirection(wow64Value);

            StringBuilder report = new StringBuilder();
            MessageBoxIcon icon = MessageBoxIcon.Information;

            AppendItems(report, "Successfully restored these plugins to their original, unpatched version:", restored);
            AppendItems(report, "These plugins could not be restored because their respective browser is currently open:", locked);
            AppendItems(report, "Caught exceptions:", errors);

            if (locked.Count > 0 || errors.Count > 0) {
                icon = MessageBoxIcon.Warning;
                report.AppendLine("Errors have been encountered during the restoration process. Please try again after reading the message above carefully.");
            } else if (restored.Count > 0) {
                report.AppendLine("All plugins have been restored from the previous backup!\nNo more action is necessary.");
            } else {
                ShowError("No backups are currently available.");
                return;
            }

            MessageBox.Show(report.ToString(), "FlashPatch!", MessageBoxButtons.OK, icon);
        }
    }
}
