using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueLib.Exceptions;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace TrueLib
{
    public class MountManager
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
        public void MountEncryptedMedia(EncryptedMedia encMedia, String encVolume, 
            Password.PasswordPromptEventHandler onPasswordPrompt = null)
        {
            StringBuilder tcArgs = new StringBuilder();
            Process tcLauncher = new Process();
            tcLauncher.StartInfo.FileName = Configuration.LauncherLocation;
            tcLauncher.StartInfo.UseShellExecute = false;

            /************************************************************************/
            /* 1. If already mounted, skip everything.                              */
            /************************************************************************/
            if (mountedVolumes.Contains(encMedia))
            {
                throw new AlreadyMountedException(encMedia + " is already mounted.");
            }

            /************************************************************************/
            /* 2. We need an unassigned drive letter.                               */
            /************************************************************************/
            if (SystemDevices.GetLogicalDisk(encMedia.Letter.Letter) != null)
            {
                throw new DriveLetterInUseException();
            }

            /************************************************************************/
            /* 3. Add mount option flags for this device.                           */
            /************************************************************************/
            tcArgs.Append(encMedia.MountOptions);

            /************************************************************************/
            /* 4. Fetch key files.                                                  */
            /************************************************************************/
            tcArgs.Append(encMedia.KeyFilesArgumentLine);

            /************************************************************************/
            /* 5. Launch pre mount programs.                                       */
            /************************************************************************/
            foreach (var preProg in encMedia.PreMountPrograms)
            {
                preProg.Launch();
            }

            /************************************************************************/
            /* 6. Try to mount with every configured password.                      */
            /************************************************************************/
            foreach (Password passwd in encMedia.Passwords)
            {
                passwd.OnPasswordPromptHandler += onPasswordPrompt;
                String tcArgsReady = string.Format("{0} /l{1} /v \"{2}\" /p \"{3}\"",
                config.TrueCrypt.CommandLineArguments,
                encMedia.Letter.Letter,
                encVolume,
                passwd.PlainPassword);

                tcLauncher.StartInfo.Arguments = string.Format("\"{0}\" {1}",
                    config.TrueCrypt.ExecutablePath, tcArgsReady);
                tcLauncher.Start();

                // Wait for incoming message
                using (NamedPipeServerStream npServer = new NamedPipeServerStream("TrueCryptMessage"))
                {
                    npServer.WaitForConnection();
                    using (StreamReader sReader = new StreamReader(npServer, Encoding.Unicode))
                    {
                        String input = sReader.ReadToEnd();

                        if (input != "OK")
                        {
                            throw new TrueCryptException(input);
                        }
                    }
                }
            }

            /************************************************************************/
            /* 7. Launch post mount programs.                                       */
            /************************************************************************/
            foreach (var postProg in encMedia.PostMountPrograms)
            {
                postProg.Launch();
            }

            /************************************************************************/
            /* 8. Open new drive in explorer.                                       */
            /************************************************************************/
            if (encMedia.OpenExplorer)
            {
                Process.Start(string.Format(@"explorer.exe {0}:\", encMedia.Letter.Current));
            }
        }

        #endregion
    }
}
