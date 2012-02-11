using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TrueLib.Remote
{
    public class ResourceLocal : RemoteResource
    {
        public LogicalDisk Device { get; set; }
        /// <summary>
        /// Returns the path to the local resource.
        /// </summary>
        public override string CachedFileName
        {
            get
            {
                if (Device != null)
                {
                    return Path.Combine(Path.GetPathRoot(Device.IdentityFile), RemotePath);
                }

                return RemotePath;
            }
        }
    }
}
