using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TrueLib.Remote
{
    public class RemoteResource : KeyItem
    {
        /// <summary>
        /// The short name of the protocol.
        /// </summary>
        public string Protocol { get; set; }
        /// <summary>
        /// Hostname or IP-Address of the destination host.
        /// </summary>
        public string Hostname { get; set; }
        /// <summary>
        /// Port number of the destination host.
        /// </summary>
        public uint Port { get; set; }
        /// <summary>
        /// Username (optional).
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password (optional).
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Absolute path in URI to the resource.
        /// </summary>
        public string RemotePath { get; set; }
        /// <summary>
        /// Local path to file (in temporary folder).
        /// </summary>
        public string LocalPath
        {
            get { return Path.Combine(Configuration.TempPath, Path.GetFileName(RemotePath)); }
        }
    }
}
