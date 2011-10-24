using System;
using System.IO;

namespace TrueLib
{
    [Serializable()]
    public class Password : RemoteResource
    {
        public string Password
        {
            get
            {
                if (this.Scheme.Equals("static", StringComparison.CurrentCultureIgnoreCase))
                {
                    // return static password
                    return this.LocalPath.Substring(1);
                }
                else
                {
                    // return content of password file
                    using (StreamReader pws = 
                        new StreamReader(this.CachedFileName, System.Text.Encoding.UTF8))
                    {
                        return pws.ReadLine();
                    }
                }
            }
        }

        public Password(string uri) : base(uri) { }
    }
}
