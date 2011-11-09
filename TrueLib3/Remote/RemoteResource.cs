using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TrueLib.Remote
{
    public class RemoteResource
    {
        /// <summary>
        /// The file name of the downloaded remote file.
        /// </summary>
        public virtual string CachedFileName { get; set; }
        public string Hostname { get; set; }
        public uint Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FilePath { get; set; }
        public string LocalPath
        {
            get { return Path.Combine(Configuration.TempPath, Path.GetFileName(FilePath)); }
        }
    }
}
