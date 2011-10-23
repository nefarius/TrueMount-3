using System;

namespace TrueLib
{
    [Serializable()]
    public class Password : Uri
    {
        public Password(string uri) : base(uri) { }
    }
}
