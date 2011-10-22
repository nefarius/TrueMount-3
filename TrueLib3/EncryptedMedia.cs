using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueLib
{
    /// <summary>
    /// Describes an encrypted media (can be a disk or a container file).
    /// </summary>
    [Serializable()]
    public class EncryptedMedia : IPhysicalMedia
    {
        public bool IsActive { get; set; }
        public bool OpenExplorer { get; set; }
        public bool ForceUnmount { get; set; }
        public List<Password> Passwords { get; set; }
        public List<KeyFile> KeyFiles { get; set; }
        public DriveLetter Letter { get; set; }
        // TODO: implement some logic to handle the dismount event (or replace it?)
        public bool TriggerDismount { get; set; }
        public List<TriggerDevice> TriggerDevices { get; set; }

        public EncryptedMedia()
        {
            this.Passwords = new List<Password>();
            this.KeyFiles = new List<KeyFile>();
            this.TriggerDevices = new List<TriggerDevice>();
        }

        /// <summary>
        /// Returns a ready-to-use command line string with chosen arguments.
        /// </summary>
        public string KeyFilesArgumentLine
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (this.KeyFiles.Count() > 0)
                {
                    foreach (KeyFile item in this.KeyFiles)
                    {
                        sb.AppendFormat("/k \"{0}\" ", item.CachedFileName);
                    }

                    return sb.ToString().Trim();
                }
                else
                    return string.Empty;
            }
        }

        public virtual bool IsOnline { get; set; }
    }
}
