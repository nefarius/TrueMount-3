using System.Text;
using System;
using System.Net;
using System.IO;
using AlexPilotti.FTPS.Client;
using AlexPilotti.FTPS.Common;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

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
                string lPath = Path.Combine(Configuration.TempPath, Path.GetFileName(this.LocalPath));

                switch ((Schemes)Enum.Parse(typeof(Schemes), this.Scheme, true))
                {
                    case Schemes.File:
                        return this.LocalPath;
                    case Schemes.HTTP:
                    case Schemes.HTTPS:
                    case Schemes.FTP:
                        using(WebClient web = new WebClient())
                        {
                            web.DownloadFile(this, lPath);
                        }
                        return lPath;
                    case Schemes.FTPeS:
                        ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertficate;
                        using(FTPSClient ftps = new FTPSClient())
                        {
                            string user = this.UserInfo.Split(':')[0];
                            string pass = this.UserInfo.Split(':')[1];
                            NetworkCredential login = new NetworkCredential(user, pass);
                            ftps.Connect(this.Host, 
                                login, 
                                ESSLSupportMode.CredentialsRequired | ESSLSupportMode.DataChannelRequested, 
                                new RemoteCertificateValidationCallback(ValidateServerCertficate));
                            ftps.GetFile(this.LocalPath, lPath);
                        }
                        return lPath;
                    default:
                        break;
                }

                throw new ArgumentException("Unsupported scheme provided!");
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

        private static bool ValidateServerCertficate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
