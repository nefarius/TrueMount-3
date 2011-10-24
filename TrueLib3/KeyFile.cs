using System.Text;
using System;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Diagnostics;
using System.Threading;

namespace TrueLib
{
    [Serializable()]
    public class KeyFile : RemoteResource
    {
        public KeyFile(string uri) : base(uri) { }
    }
}
