using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueLib.Remote
{
    public class ResourceFTP : ResourceHTTP
    {
        public ResourceFTP()
        {
            Protocol = "ftp";
            Port = 21;
        }
    }
}
