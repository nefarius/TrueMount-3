using System.Text;
using System;

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
                // TODO
                return this.LocalPath;
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
        public static KeyFile BuildUri(Shemes sheme, string host, string path)
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
        public static KeyFile BuildUri(Shemes sheme, string host, string path, string user, string pass)
        {
            return new KeyFile(string.Format("{0}://{1}:{2}@{3}/{4}",
                sheme, user, pass, host, path));
        }
    }
}
