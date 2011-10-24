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
    public class KeyFile : Uri
    {
        /// <summary>
        /// The file name of the key file's local copy.
        /// </summary>
        public string CachedFileName
        {
            get
            {
                return RemoteResource.FetchResource(this, _hostFingerprint);
            }
        }

        private string _hostFingerprint = string.Empty;
        public string HostFingerprint
        {
            get
            {
                if (this.Scheme.Equals(Schemes.SFTP.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    return _hostFingerprint;
                else
                    return null;
            }
            set
            {
                if (this.Scheme.Equals(Schemes.SFTP.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    _hostFingerprint = value;
                else
                    throw new ArgumentException("The current scheme doesn't match the required protocol SFTP.");
            }
        }

        public KeyFile(string uri) : base(uri) { }

        /// <summary>
        /// Creates a new key file.
        /// </summary>
        /// <param name="sheme">The sheme of the source.</param>
        /// <param name="host">The hostname.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static KeyFile BuildUri(Schemes sheme, string host, string path)
        {
            return new KeyFile(string.Format("{0}://{1}/{2}",
                sheme, host, path));
        }

        /// <summary>
        /// Creates a new key file with authentication information.
        /// </summary>
        /// <param name="sheme"></param>
        /// <param name="host"></param>
        /// <param name="path"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static KeyFile BuildUri(Schemes sheme, string host, string path, string user, string pass)
        {
            return new KeyFile(string.Format("{0}://{1}:{2}@{3}/{4}",
                sheme, user, pass, host, path));
        }
    }
}
