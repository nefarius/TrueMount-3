using System;
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
using System.Threading;

namespace TrueLib
{
    [Serializable()]
    public class Configuration
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
        public bool CheckForUpdates { get; set; }
        public bool WarnOnExit { get; set; }

        private const string RUN_LOCATION = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string PRODUCT_NAME = "TrueMount 3";
        private const string CONFIG_FILEN = "config.xml";
        #endregion

        public Configuration()
        {
            // initiate empty references
            TrueCrypt = new TrueCryptConfig();
            TriggerDevices = new List<TriggerDevice>();
            EncryptedDisks = new List<EncryptedDisk>();
            EncryptedPartitions = new List<EncryptedPartition>();
            EncryptedContainerFiles = new List<EncryptedContainerFile>();
            Language = Thread.CurrentThread.CurrentUICulture;
            
            // set default values
            BalloonTimePeriod = 3000;
            FirstStart = true;
            CheckForUpdates = true;
            WarnOnExit = true;
        }

        /// <summary>
        /// Full path to TrueCrypt IPC launcher.
        /// </summary>
        public static string LauncherLocation
        {
            get { return Path.Combine(AssemblyPath, "TCLauncher.exe"); }
        }

        /// <summary>
        /// Path to updater assembly.
        /// </summary>
        public static string UpdaterLocation
        {
            get { return Path.Combine(AssemblyPath, "updater.exe"); }
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
        public static string AssemblyLocation
        {
            get { return Assembly.GetExecutingAssembly().Location; }
        }

        /// <summary>
        /// Path name of the current running assembly instance.
        /// </summary>
        public static string AssemblyPath
        {
            get { return Path.GetDirectoryName(AssemblyLocation); }
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
                return Path.Combine(ConfigurationPath, CONFIG_FILEN);
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
            key.SetValue(PRODUCT_NAME, AssemblyLocation);
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
                return (value == AssemblyLocation);
            }
        }

        /// <summary>
        /// Gets or sets the locally stored configuration.
        /// </summary>
        public static Configuration Local
        {
            get
            {
                Configuration stored = new Configuration();

                if (File.Exists(ConfigurationFile))
                {
                    using (FileStream fsFetch = new FileStream(ConfigurationFile, FileMode.Open))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        stored = (Configuration)bf.Deserialize(fsFetch);
                    }
                }

                return stored;
            }

            set
            {
                using (FileStream fsSave = new FileStream(ConfigurationFile, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fsSave, value);
                }
            }
        }
    }
}
