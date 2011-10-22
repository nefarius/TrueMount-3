using System;
using System.Xml.Serialization;

namespace TrueLib
{
    [Serializable()]
    public class Password
    {
        private string _uri = string.Empty;
        [XmlIgnore]
        public Uri Location
        {
            get { return new Uri(_uri); }
            set { _uri = value.ToString(); }
        }

        private Password() { }
        public Password(string uri) { Location = new Uri(uri); }
        public Password(Uri uri) { Location = uri; }
    }
}
