using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueLib.Exceptions;

namespace TrueLib
{
    class MountManager
    {
        private List<EncryptedMedia> mountedVolumes = new List<EncryptedMedia>();
        private Configuration config = null;

        public MountManager(Configuration config)
        {
            this.config = config;
        }

        #region Mount and Unmount methods
        /*
        /// <summary>
        /// Reads configuration and tries to mount every found device.
        /// </summary>
        /// <returns>Returns count of mounted SystemDevices, if none returns zero.</returns>
        private int MountAllDevices()
        {
            int mountedPartitions = 0;

            // this method can't do very much without partitions
            if (config.EncryptedDiskPartitions.Count <= 0)
            {
                LogAppend("WarnNoDisks");
                return mountedPartitions;
            }

            // walk through every partition in configuration
            foreach (EncryptedDiskPartition enc_disk_partition in config.EncryptedDiskPartitions)
                if (MountPartition(enc_disk_partition))
                    mountedPartitions++;

            // walk through every container file in configuration
            foreach (EncryptedContainerFile encContainerFile in config.EncryptedContainerFiles)
                if (MountContainerFile(encContainerFile))
                    mountedPartitions++;

            LogAppend("MountedPartitions", mountedPartitions.ToString());
            return mountedPartitions;
        }

        /// <summary>
        /// Mount a specific encrypted partition.
        /// </summary>
        /// <param name="encDiskPartition">The encrypted partition to mount.</param>
        /// <returns>Returns true on successful mount, else false.</returns>
        private bool MountPartition(EncryptedDiskPartition encDiskPartition)
        {
            bool mountSuccess = false;

            LogAppend("SearchDiskLocal");

            // is the partition marked as active?
            if (!encDiskPartition.IsActive)
            {
                // log and skip disk if marked as inactive
                LogAppend("DiskDriveConfDisabled", encDiskPartition.DiskCaption);
                return mountSuccess;
            }
            else
                LogAppend("DiskConfEnabled", encDiskPartition.DiskCaption);

            // find local disk
            ManagementObject diskPhysical =
                SystemDevices.GetDiskDriveBySignature(encDiskPartition.DiskCaption,
                    encDiskPartition.DiskSignature);

            // is the disk online? if not, skip it
            if (diskPhysical == null)
            {
                // disk is offline, log and skip
                LogAppend("DiskDriveOffline", encDiskPartition.DiskCaption);
                return mountSuccess;
            }
            else
                LogAppend("DiskIsOnline", diskPhysical["Caption"].ToString());

            // get the index of the parent disk
            uint diskIndex = uint.Parse(diskPhysical["Index"].ToString());
            // get the index of this partition ("real" index is zero-based)
            uint partIndex = encDiskPartition.PartitionIndex - 1;

            // get original device id from local disk
            String deviceId = null;
            try
            {
                if (encDiskPartition.PartitionIndex > 0)
                    deviceId = SystemDevices.GetPartitionByIndex(diskIndex, partIndex)["DeviceID"].ToString();
                else
                    deviceId = SystemDevices.GetTCCompatibleDiskPath(diskIndex);
            }
            catch (NullReferenceException)
            {
                LogAppend("ErrVolumeOffline", encDiskPartition.ToString());
                return mountSuccess;
            }

            LogAppend("DiskDeviceId", deviceId);

            // convert device id in truecrypt compatible name
            String tcDevicePath = SystemDevices.GetTCCompatibleName(deviceId);
            LogAppend("DiskDrivePath", tcDevicePath);

            // try to mount and return true on success
            return MountEncryptedMedia(encDiskPartition, tcDevicePath);
        }

        /// <summary>
        /// Mount a specific container file.
        /// </summary>
        /// <param name="containerFile">The container file to mount.</param>
        /// <returns>Returns true on successful mount, else false.</returns>
        private bool MountContainerFile(EncryptedContainerFile containerFile)
        {
            bool mountSuccess = false;

            LogAppend("SearchConFile");

            // skip the file if inactive
            if (!containerFile.IsActive)
            {
                LogAppend("ConConfDisabled", containerFile.FileName);
                return mountSuccess;
            }
            else
                LogAppend("ConConfEnabled", containerFile.FileName);

            // check if file exists on local system
            if (!File.Exists(containerFile.FileName))
            {
                LogAppend("ErrConFileNotExists", containerFile.FileName);
                return mountSuccess;
            }
            else
                LogAppend("ConFileFound", containerFile.FileName);

            // try to mount the volume and return true on success
            return MountEncryptedMedia(containerFile, containerFile.FileName);
        }
        */

        /// <summary>
        /// Mounts a specific media.
        /// </summary>
        /// <param name="encMedia">The encrypted media to mount.</param>
        /// <param name="encVolume">The device path or file name to mount.</param>
        /// <returns>Returns true on successful mount, else false.</returns>
        private void MountEncryptedMedia(EncryptedMedia encMedia, String encVolume)
        {
            // if already mounted skip everything
            if (mountedVolumes.Contains(encMedia))
            {
                throw new AlreadyMountedException(encMedia + " is already mounted.");
            }

            // local drive letter must not be assigned!
            if (SystemDevices.GetLogicalDisk(encMedia.Letter.Letter) != null)
            {
                throw new DriveLetterInUseException();
            }

            // fill in the attributes we got above
            String tcArgsReady = string.Format("{0} /l{1} /v \"{2}\" /p \"{3}\"",
                config.TrueCrypt.CommandLineArguments,
                encMedia.Letter.Letter,
                encVolume,
                encMedia)
                
                config.TrueCrypt.CommandLineArguments +
                "/l" + encMedia. +
                " /v \"" + encVolume + "\"" +
                " /p \"" + password + "\"";
            // unset password (it's now in the argument line)
            password = null;
#if DEBUG
            LogAppend(null, "Full argument line: {0}", tcArgsReady);
#endif

            // add specified mount options to argument line
            if (!string.IsNullOrEmpty(encMedia.MountOptions))
            {
                LogAppend("AddMountOpts", encMedia.MountOptions);
                tcArgsReady += " " + encMedia.MountOptions;
#if DEBUG
                LogAppend(null, "Full argument line: {0}", tcArgsReady);
#endif
            }
            else
                LogAppend("NoMountOpts");

            // add key files
            if (!string.IsNullOrEmpty(encMedia.KeyFilesArgumentLine))
            {
                LogAppend("AddKeyFiles", encMedia.KeyFiles.Count.ToString());
                tcArgsReady += " " + encMedia.KeyFilesArgumentLine;
#if DEBUG
                LogAppend(null, "Full argument line: {0}", tcArgsReady);
#endif
            }
            else
                LogAppend("NoKeyFiles");

            // if not exists, exit
            if (string.IsNullOrEmpty(config.TrueCrypt.ExecutablePath))
            {
                // password is in here, so free it
                tcArgsReady = null;
                // damn just a few more steps! -.-
                LogAppend("ErrTCNotFound");
                buttonStartWorker.Enabled = false;
                buttonStopWorker.Enabled = false;
                LogAppend("CheckCfgTC");
                return mountSuccess;
            }
            else
                LogAppend("TCPath", config.TrueCrypt.ExecutablePath);

            // create new process
            Process tcLauncher = new Process();
            // set exec name
            tcLauncher.StartInfo.FileName = Configuration.LauncherLocation;
            // set arguments
            tcLauncher.StartInfo.Arguments = '"' + config.TrueCrypt.ExecutablePath +
                "\" " + tcArgsReady;
            // use CreateProcess()
            tcLauncher.StartInfo.UseShellExecute = false;
#if DEBUG
            LogAppend(null, "StartInfo.Arguments: {0}", tcLauncher.StartInfo.Arguments);
#endif

            // arr, fire the canon! - well, try it...
            try
            {
                LogAppend("StartProcess");
                tcLauncher.Start();
            }
            catch (Win32Exception ex)
            {
                // dammit, dammit, dammit! something went wrong at the very end...
                LogAppend("ErrGeneral", ex.Message);
                buttonStartWorker.Enabled = false;
                buttonStopWorker.Enabled = false;
                LogAppend("CheckTCConf");
                return mountSuccess;
            }
            LogAppend("ProcessStarted");

            // Status
            LogAppend("WaitDevLaunch");
            Cursor.Current = Cursors.WaitCursor;

            // Wait for incoming message
            using (NamedPipeServerStream npServer = new NamedPipeServerStream("TrueCryptMessage"))
            {
                npServer.WaitForConnection();
                using (StreamReader sReader = new StreamReader(npServer, Encoding.Unicode))
                {
                    String input = sReader.ReadToEnd();
#if DEBUG
                    LogAppend(null, "Pipe: {0}", input);
#endif

                    if (input != "OK")
                    {
                        LogAppend("ErrTrueCryptMsg", input);
                        if (config.TrueCrypt.ShowErrors)
                        {
#if DEBUG
                            MessageBox.Show(string.Format(langRes.GetString("MsgTDiskTimeout"), encMedia, input),
                                                            langRes.GetString("MsgHDiskTimeout"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif

                            notifyIconSysTray.BalloonTipTitle = langRes.GetString("MsgHDiskTimeout");
                            notifyIconSysTray.BalloonTipIcon = ToolTipIcon.Warning;
                            notifyIconSysTray.BalloonTipText = string.Format(langRes.GetString("MsgTDiskTimeout"), encMedia, input);
                            notifyIconSysTray.ShowBalloonTip(config.BalloonTimePeriod);
                        }

                        LogAppend("MountCanceled", encMedia.ToString());
                        Cursor.Current = Cursors.Default;
                        return mountSuccess;
                    }
                    else
                        LogAppend("InfoLauncherOk");
                }
            }

            Cursor.Current = Cursors.Default;

            LogAppend("LogicalDiskOnline", encMedia.DriveLetterCurrent);
            // mount was successful
            mountSuccess = true;
            // display balloon tip on successful mount
            MountBalloonTip(encMedia);

            // if set, open device content in windows explorer
            if (encMedia.OpenExplorer)
            {
                LogAppend("OpenExplorer", encMedia.DriveLetterCurrent);
                try
                {
                    Process.Start("explorer.exe", encMedia.DriveLetterCurrent + @":\");
                }
                catch (Exception eex)
                {
                    // error in windows explorer (what a surprise)
                    LogAppend("ErrGeneral", eex.Message);
                    LogAppend("ErrExplorerOpen");
                }
            }

            // add the current mounted media to the dismount sys tray list
            AddMountedMedia(encMedia);

            return mountSuccess;
        }

        #endregion
    }
}
