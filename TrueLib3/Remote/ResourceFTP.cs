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
            // Basically the same like HTTP, just different sheme and port
            Protocol = "ftp";
            Port = 21;
        }
    }
}
