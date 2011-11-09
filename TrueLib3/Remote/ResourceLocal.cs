using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueLib.Remote
{
    public class ResourceLocal : RemoteResource
    {
        public override string CachedFileName
        {
            get { return FilePath; }
        }
    }
}
