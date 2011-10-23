using System.Text;
using System;
using System.Net;
using System.IO;
using AlexPilotti.FTPS.Client;
using AlexPilotti.FTPS.Common;
using System.Security.Cryptography.X509Certificates;
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
                string lPath = Path.Combine(Configuration.TempPath, Path.GetFileName(this.LocalPath));
                string user = this.UserInfo.Split(':')[0];
                string pass = this.UserInfo.Split(':')[1];

                switch ((Schemes)Enum.Parse(typeof(Schemes), this.Scheme, true))
                {
                    case Schemes.File:
                        return this.LocalPath;
                    case Schemes.HTTP:
                    case Schemes.HTTPS:
                    case Schemes.FTP:
                        using (WebClient web = new WebClient())
                        {
                            web.DownloadFile(this, lPath);
                        }
                        return lPath;
                    case Schemes.FTPeS:
                        ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertficate;
                        using (FTPSClient ftps = new FTPSClient())
                        {
                            NetworkCredential login = new NetworkCredential(user, pass);
                            ftps.Connect(this.Host,
                                login,
                                ESSLSupportMode.CredentialsRequired | ESSLSupportMode.DataChannelRequested,
                                new RemoteCertificateValidationCallback(ValidateServerCertficate));
                            ftps.GetFile(this.LocalPath, lPath);
                        }
                        return lPath;
                    case Schemes.SFTP:
                        Process winscp = new Process();
                        winscp.StartInfo.FileName = "winscp.com";
                        winscp.StartInfo.UseShellExecute = false;
                        winscp.StartInfo.CreateNoWindow = true;
                        winscp.StartInfo.RedirectStandardInput = true;
                        winscp.StartInfo.RedirectStandardOutput = true;
                        winscp.Start();

                        winscp.StandardInput.WriteLine("option batch abort");
                        winscp.StandardInput.WriteLine("option confirm off");
                        // establish connection to server
                        string open = string.Format("open -hostkey=\"{2}\" {0}@{1}", user, this.Host, _hostFingerprint);
                        winscp.StandardInput.WriteLine(open);
                        // send password
                        winscp.StandardInput.WriteLine(pass);
                        // change to desired directory on server
                        string cd = string.Format("cd {0}", Path.GetDirectoryName(this.LocalPath).Replace('\\', '/'));
                        winscp.StandardInput.WriteLine(cd);
                        // set transfer mode
                        winscp.StandardInput.WriteLine("option transfer binary");
                        // download the file to local directory
                        string get = string.Format("get \"{0}\" \"{1}\"", Path.GetFileName(this.LocalPath), lPath);
                        winscp.StandardInput.WriteLine(get);
                        // close the session
                        winscp.StandardInput.WriteLine("close");
                        // exit WinSCP
                        winscp.StandardInput.WriteLine("exit");

                        // close input stream
                        winscp.StandardInput.Close();
                        string output = winscp.StandardOutput.ReadToEnd();

                        // Wait for process to completely shut down
                        winscp.WaitForExit();
                        return lPath;
                    default:
                        break;
                }

                throw new ArgumentException("Unsupported scheme provided!");
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

        // TODO: add prompt handler!
        private static bool ValidateServerCertficate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
