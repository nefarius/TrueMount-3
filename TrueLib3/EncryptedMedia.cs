﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueLib.Remote;

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
        public List<RemoteResource> Passwords { get; set; }
        public List<RemoteResource> KeyFiles { get; set; }
        public DriveLetter Letter { get; set; }
        // TODO: implement some logic to handle the dismount event (or replace it?)
        public bool TriggerDismount { get; set; }
        public List<TriggerDevice> TriggerDevices { get; set; }
        public List<Program> PreMountPrograms { get; set; }
        public List<Program> PostMountPrograms { get; set; }
        public MountOptions MountOptions { get; set; }

        public EncryptedMedia()
        {
            this.Passwords = new List<RemoteResource>();
            this.KeyFiles = new List<RemoteResource>();
            this.TriggerDevices = new List<TriggerDevice>();
            this.PreMountPrograms = new List<Program>();
            this.PostMountPrograms = new List<Program>();
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
                    foreach (var item in this.KeyFiles)
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
