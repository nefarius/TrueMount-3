using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueLib.Remote
{
    public class ResourceLocal : RemoteResource
    {
        /// <summary>
        /// Returns the path to the local resource.
        /// </summary>
        public override string CachedFileName
        {
            get { return RemotePath; }
        }
    }
}
