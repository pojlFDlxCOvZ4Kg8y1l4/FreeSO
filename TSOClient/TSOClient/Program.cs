/*The contents of this file are subject to the Mozilla Public License Version 1.1
(the "License"); you may not use this file except in compliance with the
License. You may obtain a copy of the License at http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS" basis,
WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License for
the specific language governing rights and limitations under the License.

The Original Code is the TSO LoginServer.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s): ______________________________________.
*/

using System;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Security;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace TSOClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread]
        static void Main(string[] args)
        {
            //Controls whether the application is allowed to start.
            bool Exit = false;
            string Software = "";

            if ((is64BitOperatingSystem == false) && (is64BitProcess == false))
                Software = "SOFTWARE";
            else
                Software = "SOFTWARE\\Wow6432Node";

            RegistryKey softwareKey = Registry.LocalMachine.OpenSubKey(Software);
            if (Array.Exists(softwareKey.GetSubKeyNames(), delegate(string s) { return s.Equals("Microsoft", StringComparison.InvariantCultureIgnoreCase); }))
            {
                RegistryKey msKey = softwareKey.OpenSubKey("Microsoft");
                if (Array.Exists(msKey.GetSubKeyNames(), delegate(string s) { return s.Equals("XNA", StringComparison.InvariantCultureIgnoreCase); }))
                {
                    RegistryKey xnaKey = msKey.OpenSubKey("XNA");
                    if (Array.Exists(xnaKey.GetSubKeyNames(), delegate(string s) { return s.Equals("Framework", StringComparison.InvariantCultureIgnoreCase); }))
                    {
                        RegistryKey asmKey = xnaKey.OpenSubKey("Framework");
                        if (!Array.Exists(asmKey.GetSubKeyNames(), delegate(string s) { return s.Equals("v3.1", StringComparison.InvariantCultureIgnoreCase); }))
                        {
                            MessageBox.Show("XNA was found to be installed on your system, but you do not have version 3.1. Please download and install XNA version 3.1.");
                        }
                    }
                    else
                        MessageBox.Show("XNA was found to be installed on your system, but certain components are missing. Please (re)download and (re)install XNA version 3.1.");
                }
                else
                    MessageBox.Show("XNA was not found to be installed on your system. Please download and install XNA version 3.1.");
            }
            else
                MessageBox.Show("Error: No Microsoft products were found on your system.");

            if (args.Length > 0)
            {
                int ScreenWidth = int.Parse(args[0].Split("x".ToCharArray())[0]);
                int ScreenHeight = int.Parse(args[0].Split("x".ToCharArray())[1]);

                if (args.Length >= 1)
                {
                    if (args[1].Equals("windowed", StringComparison.InvariantCultureIgnoreCase))
                        GlobalSettings.Default.Windowed = true;
                }
            }

            //Find the path to TSO on the user's system.
            softwareKey = Registry.LocalMachine.OpenSubKey("SOFTWARE");

            if (Array.Exists(softwareKey.GetSubKeyNames(), delegate(string s) { return s.Equals("Maxis", StringComparison.InvariantCultureIgnoreCase); }))
            {
                RegistryKey maxisKey = softwareKey.OpenSubKey("Maxis");
                if (Array.Exists(maxisKey.GetSubKeyNames(), delegate(string s) { return s.Equals("The Sims Online", StringComparison.InvariantCultureIgnoreCase); }))
                {
                    RegistryKey tsoKey = maxisKey.OpenSubKey("The Sims Online");
                    string installDir = (string)tsoKey.GetValue("InstallDir");
                    installDir += "\\TSOClient\\";
                    GlobalSettings.Default.StartupPath = installDir;
                }
                else
                {
                    MessageBox.Show("Error TSO was not found on your system.");
                    Exit = true;
                }
            }
            else
            {
                MessageBox.Show("Error: No Maxis products were found on your system.");
                Exit = true;
            }

            if (!Exit)
            {
                using (Game1 game = new Game1())
                {
                    GlobalSettings.Default.ClientVersion = GetClientVersion();
                    //This path should be used to store all files generated by the client, to avoid access conflicts.
                    GlobalSettings.Default.DocumentsPath = 
                        System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Project Dollhouse\\";

                    if(!Directory.Exists(GlobalSettings.Default.DocumentsPath))
                        Directory.CreateDirectory(GlobalSettings.Default.DocumentsPath);

                    game.Run();
                }
            }
        }
        
        /// <summary>
        /// Determines whether or not the program is being run as an administrator.
        /// </summary>
        private static bool IsAdministrator
        {
            get
            {
                WindowsIdentity wi = WindowsIdentity.GetCurrent();
                WindowsPrincipal wp = new WindowsPrincipal(wi);

                return wp.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private static bool is64BitProcess = (IntPtr.Size == 8);
        private static bool is64BitOperatingSystem = is64BitProcess || InternalCheckIsWow64();

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        public static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal))
                    {
                        return false;
                    }
                    return retVal;
                }
            }
                else
                {
                return false;
            }
        }

        /// <summary>
        /// Loads the client's version from "Client.manifest".
        /// This is here because it should be one of the first
        /// things the client does when it starts.
        /// </summary>
        /// <returns>The version.</returns>
        private static string GetClientVersion()
        {
            string ExeDir = GlobalSettings.Default.StartupPath;

            //Never make an assumption that a file exists.
            if (File.Exists(ExeDir + "\\Client.manifest"))
            {
                using(BinaryReader Reader = new BinaryReader(File.Open(ExeDir + "\\Client.manifest", FileMode.Open)))
                {
                    return Reader.ReadString() + ".0"; //Last version number is unused.
                }
            }
            else
            {
                //Version as of writing this method.
                return "0.1.22.0";
            }
        }
    }
}

