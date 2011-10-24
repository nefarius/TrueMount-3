using System;

namespace TrueLib
{
    [Serializable()]
    public class KeyFile : RemoteResource
    {
        public KeyFile(string uri) : base(uri) { }
    }
}
