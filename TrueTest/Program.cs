using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueLib;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Management;
using System.Runtime.InteropServices;

namespace TrueTest
{
    class Program
    {
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        extern static bool GetVolumeInformation(
          string RootPathName,
          StringBuilder VolumeNameBuffer,
          int VolumeNameSize,
          out uint VolumeSerialNumber,
          out uint MaximumComponentLength,
          out FileSystemFeature FileSystemFlags,
          StringBuilder FileSystemNameBuffer,
          int nFileSystemNameSize);

        [Flags]
        public enum FileSystemFeature : uint
        {
            /// <summary>
            /// The file system supports case-sensitive file names.
            /// </summary>
            CaseSensitiveSearch = 1,
            /// <summary>
            /// The file system preserves the case of file names when it places a name on disk.
            /// </summary>
            CasePreservedNames = 2,
            /// <summary>
            /// The file system supports Unicode in file names as they appear on disk.
            /// </summary>
            UnicodeOnDisk = 4,
            /// <summary>
            /// The file system preserves and enforces access control lists (ACL).
            /// </summary>
            PersistentACLS = 8,
            /// <summary>
            /// The file system supports file-based compression.
            /// </summary>
            FileCompression = 0x10,
            /// <summary>
            /// The file system supports disk quotas.
            /// </summary>
            VolumeQuotas = 0x20,
            /// <summary>
            /// The file system supports sparse files.
            /// </summary>
            SupportsSparseFiles = 0x40,
            /// <summary>
            /// The file system supports re-parse points.
            /// </summary>
            SupportsReparsePoints = 0x80,
            /// <summary>
            /// The specified volume is a compressed volume, for example, a DoubleSpace volume.
            /// </summary>
            VolumeIsCompressed = 0x8000,
            /// <summary>
            /// The file system supports object identifiers.
            /// </summary>
            SupportsObjectIDs = 0x10000,
            /// <summary>
            /// The file system supports the Encrypted File System (EFS).
            /// </summary>
            SupportsEncryption = 0x20000,
            /// <summary>
            /// The file system supports named streams.
            /// </summary>
            NamedStreams = 0x40000,
            /// <summary>
            /// The specified volume is read-only.
            /// </summary>
            ReadOnlyVolume = 0x80000,
            /// <summary>
            /// The volume supports a single sequential write.
            /// </summary>
            SequentialWriteOnce = 0x100000,
            /// <summary>
            /// The volume supports transactions.
            /// </summary>
            SupportsTransactions = 0x200000,
        }

        static void Main(string[] args)
        {
#if TEST1
            KeyFile kf1 = new KeyFile("file://C:/temp/kf1.txt");
            KeyFile kf2 = new KeyFile("https://nefarius.at/private/kf2.txt");
            KeyFile kf3 = new KeyFile("sftp://nefarius@dhmx.at/kf3.txt");

            EncryptedMedia em1 = new EncryptedMedia();
            em1.KeyFiles.Add(kf1);
            em1.KeyFiles.Add(kf2);
            em1.KeyFiles.Add(kf3);

            // Print Keyfiles
            foreach (KeyFile item in em1.KeyFiles)
            {
                Console.WriteLine(Path.GetFullPath(item.LocalPath));
            }

            Console.WriteLine(em1.KeyFilesArgumentLine);
            Console.WriteLine();
            
            // BuildUri
            KeyFile kf4 = KeyFile.BuildUri(Shemes.HTTPS, "nefarius.at", @"private/lulz.txt");

            ManagementPath mpath = new ManagementPath("Win32_LogicalDisk.DeviceID=\"C:\"");
            ManagementObject mobj = new ManagementObject(mpath);

            // Serialization
            BinaryFormatter serializer = new BinaryFormatter();
            FileStream fs = new FileStream("stored.bin", FileMode.Create);
            serializer.Serialize(fs, em1);
            fs.Close();

            MountOptions mo = new MountOptions();
            mo.Removable = true;
            mo.System = true;
            Console.WriteLine("MO: '{0}'", mo.MountOptionsArgumentLine);

            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo di in drives)
                Console.WriteLine("Drive: {0}", di);

            StringBuilder volname = new StringBuilder(261);
            StringBuilder fsname = new StringBuilder(261);
            uint sernum, maxlen;
            FileSystemFeature flags;
            if (!GetVolumeInformation("c:\\", volname, volname.Capacity, out sernum, out maxlen, out flags, fsname, fsname.Capacity))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            string volnamestr = volname.ToString();
            string fsnamestr = fsname.ToString();

            Console.WriteLine("Volname: {0}, FSName: {1}", volnamestr, fsnamestr);

            ManagementClass w32ldtp = new ManagementClass("Win32_LogicalDiskToPartition");
            ManagementClass w32dd = new ManagementClass("Win32_DiskDrive");
#endif
            EncryptedPartition ep1 = new EncryptedPartition("PHYSICALDRIVE0", 1236457623, 0);
            EncryptedPartition ep2 = new EncryptedPartition("PHYSICALDRIVE0", 1236457623, 2);

            Console.WriteLine(ep1.Equals(ep2));
            Console.WriteLine(ep1.ToString());

            EncryptedPartition part1 = new EncryptedPartition("HITACHI HTS545050B9A300", 3288318975, 0);
            Console.WriteLine("{0} is {1}", part1, part1.IsOnline());

            Console.ReadKey();
        }
    }
}
