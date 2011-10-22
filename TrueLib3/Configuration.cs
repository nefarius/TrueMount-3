﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace TrueLib
{
    [Serializable()]
    class Configuration
    {
        #region Definitions, Variables
        public bool FirstStart { get; set; }
        public bool AutostartService { get; set; }
        public bool StartSilent { get; set; }
        public TrueCryptConfig TrueCrypt { get; set; }
        public List<TriggerDevice> TriggerDevices { get; set; }
        public List<EncryptedDisk> EncryptedDisks { get; set; }
        public List<EncryptedPartition> EncryptedPartitions { get; set; }
        public List<EncryptedContainerFile> EncryptedContainerFiles { get; set; }
        public CultureInfo Language { get; set; }
        public bool IgnoreAssignedDriveLetters { get; set; }
        public bool ForceUnmount { get; set; }
        public bool UnmountWarning { get; set; }
        public bool DisableBalloons { get; set; }
        public int BalloonTimePeriod { get; set; }
        public string ApplicationLocation { get; set; }
        public bool CheckForUpdates { get; set; }
        public bool WarnOnExit { get; set; }

        private const string RUN_LOCATION = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string PRODUCT_NAME = "TrueMount 3";
        #endregion

        public Configuration()
        {
            // initiate empty references
            TrueCrypt = new TrueCryptConfig();
            KeyDevices = new List<UsbKeyDevice>();
            EncryptedDiskPartitions = new List<EncryptedDiskPartition>();
            EncryptedContainerFiles = new List<EncryptedContainerFile>();
            
            // set default values
            OnlyOneInstance = true;
            BalloonTimePeriod = 3000;
            FirstStart = true;
            ApplicationLocation = CurrentApplicationLocation;
            CheckForUpdates = true;
            WarnOnExit = true;
        }

        /// <summary>
        /// Full path to TrueCrypt IPC launcher.
        /// </summary>
        public static string LauncherLocation
        {
            get { return Path.Combine(CurrentApplicationPath, "TCLauncher.exe"); }
        }

        /// <summary>
        /// Path to updater assembly.
        /// </summary>
        public static string UpdaterLocation
        {
            get { return Path.Combine(CurrentApplicationPath, "updater.exe"); }
        }

        /// <summary>
        /// Checks if updater component exists in applications working directory.
        /// </summary>
        public static bool UpdaterExists
        {
            get { return File.Exists(UpdaterLocation); }
        }

        /// <summary>
        /// Returns the current running assembly version information.
        /// </summary>
        public static Version CurrentVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        /// <summary>
        /// Full name of the current running assembly instance.
        /// </summary>
        public static string CurrentApplicationLocation
        {
            get { return Assembly.GetExecutingAssembly().Location; }
        }

        /// <summary>
        /// Path name of the current running assembly instance.
        /// </summary>
        public static string CurrentApplicationPath
        {
            get { return Path.GetDirectoryName(CurrentApplicationLocation); }
        }

        /// <summary>
        /// URL to the projects web page.
        /// </summary>
        public static string ProjectLocation
        {
            get { return "http://nefarius.at/windows/truemount2"; }
        }

        /// <summary>
        /// Contains the path where the configuration file and updates are stored.
        /// </summary>
        public static string ConfigurationPath
        {
            get
            {
                string appDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PRODUCT_NAME);
#if DEBUG
                appDataDir = Path.Combine(appDataDir, "debug");
#endif
                if (!Directory.Exists(appDataDir))
                    Directory.CreateDirectory(appDataDir);
                return appDataDir;
            }
        }

        /// <summary>
        /// Contains the path of the configuration file.
        /// </summary>
        public static string ConfigurationFile
        {
            get
            {
                return Path.Combine(ConfigurationPath, "config.dat");
            }
        }

        /// <summary>
        /// The Name of the Application.
        /// </summary>
        public string ApplicationName
        {
            get { return PRODUCT_NAME; }
        }

        /// <summary>
        /// Add to windows autostart.
        /// </summary>
        public void SetAutoStart()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RUN_LOCATION);
            key.SetValue(PRODUCT_NAME, CurrentApplicationLocation);
        }

        /// <summary>
        /// Remove from windows autostart.
        /// </summary>
        public void UnSetAutoStart()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RUN_LOCATION);
            key.DeleteValue(PRODUCT_NAME);
        }

        /// <summary>
        /// Checks if application is in autostart list.
        /// </summary>
        public bool IsAutoStartEnabled
        {
            get
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(RUN_LOCATION);
                if (key == null)
                    return false;

                string value = (string)key.GetValue(PRODUCT_NAME);
                if (value == null)
                    return false;
                return (value == CurrentApplicationLocation);
            }
        }

        /// <summary>
        /// Saves the given configuration to file.
        /// </summary>
        /// <param name="current">The current active configuration object.</param>
        public static void SaveConfiguration(Configuration current)
        {
            using(FileStream fsSave = new FileStream(ConfigurationFile, FileMode.Create))
            {
                XmlSerializer xml = new XmlSerializer(typeof(Configuration));
                xml.Serialize(fsSave, current);
            }
        }

        /// <summary>
        /// Opens the configuration from file.
        /// </summary>
        /// <returns>Returns stored or new empty default configuration reference.</returns>
        public static Configuration LoadConfiguration()
        {
            Configuration stored = new Configuration();

            if (File.Exists(ConfigurationFile))
            {
                using (FileStream fsFetch = new FileStream(ConfigurationFile, FileMode.Open))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(Configuration));
                    stored = (Configuration)xml.Deserialize(fsFetch);
                }
            }

            return stored;
        }

        /// <summary>
        /// Tries to start an updater instance and waits for its response.
        /// </summary>
        /// <param name="silent">Set true if you want to suppress dialogs.</param>
        /// <returns>Returns true on successful update, else false.</returns>
        public bool InvokeUpdateProcess(bool silent = false)
        {
            string lastAppStartPath = Path.GetDirectoryName(this.ApplicationLocation);
            // valid paths are needed to start the updater
            if (!string.IsNullOrEmpty(lastAppStartPath) && !string.IsNullOrEmpty(UpdateSavePath))
            {
                try
                {
                    Process updater = Process.Start(UpdaterLocation);

                    using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("TrueMountUpdater"))
                    {
                        pipeServer.WaitForConnection();

                        using (StreamWriter outStream = new StreamWriter(pipeServer))
                        {
                            // silent mode?
                            outStream.WriteLine(silent);
                            // directory to store update in
                            outStream.WriteLine(Configuration.UpdateSavePath);
                            // directory to patch
                            outStream.WriteLine(lastAppStartPath);
                            // go!
                            outStream.Flush();
                        }
                    }

                    // wait for updater to finish and continue or exit
                    while (!updater.WaitForExit(1000)) ;
                    if (updater.ExitCode != 2)
                        Environment.Exit(0);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if one or more volumes need a password dialog.
        /// </summary>
        public bool IsUserPasswordNeeded
        {
            get
            {
                foreach (EncryptedMedia encMedia in this.EncryptedDiskPartitions)
                {
                    if (encMedia.FetchUserPassword)
                        return true;
                }

                foreach (EncryptedMedia encMedia in this.EncryptedContainerFiles)
                {
                    if (encMedia.FetchUserPassword)
                        return true;
                }

                return false;
            }
        }
    }
}
