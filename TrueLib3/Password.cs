using System;

namespace TrueLib
{
    [Serializable()]
    public class Password : RemoteResource
    {
        public Password(string uri) : base(uri) { }
    }
}
