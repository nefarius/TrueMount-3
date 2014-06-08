using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TrueLib.Remote
{
    public class ResourceLocal : KeyItem
    {
        public LogicalDisk Device { get; set; }
        public string FilePath { get; set; }
        /// <summary>
        /// Returns the path to the local resource.
        /// </summary>
        public override string CachedFileName
        {
            get
            {
                if (Device != null)
                {
                    return Path.Combine(Path.GetPathRoot(Device.IdentityFile), FilePath);
                }

                return FilePath;
            }
        }
    }
}
