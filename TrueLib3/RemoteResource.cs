using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using AlexPilotti.FTPS.Client;
using TrueLib.Exceptions;
using net.kvdb.webdav;
using System.Threading;

namespace TrueLib
{
    public class RemoteResource : Uri
    {
        public RemoteResource(string uri) : base(uri) { }

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

        /// <summary>
        /// The file name of the key file's local copy.
        /// </summary>
        public string CachedFileName
        {
            get
            {
                string lPath = Path.Combine(Configuration.TempPath, Path.GetFileName(this.LocalPath));
                string user = string.Empty;
                string pass = string.Empty;
                if (!string.IsNullOrEmpty(this.UserInfo))
                {
                    user = this.UserInfo.Split(':')[0];
                    pass = this.UserInfo.Split(':')[1];
                }

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

                        // Wait for process to completely shut down
                        winscp.WaitForExit();

                        if (!File.Exists(lPath))
                        {
                            throw new WinSCPException(winscp.StandardOutput.ReadToEnd());
                        }

                        return lPath;
                    case Schemes.WebDAV:
                    case Schemes.WebDAVS:
                        AutoResetEvent autoResetEvent = new AutoResetEvent(false);
                        DownloadCompleteDel dav_ListComplete = delegate
                        {
                            autoResetEvent.Set();
                        };
                        WebDAVClient dav = new WebDAVClient();
                        dav.DownloadComplete += new DownloadCompleteDel(dav_ListComplete);
                        dav.Server = string.Format((this.Scheme.Equals(Schemes.WebDAV.ToString(),
                            StringComparison.CurrentCultureIgnoreCase)) ? "http://{0}" : "https://{0}", 
                            this.Host);
                        dav.Port = (this.Port == -1) ? 80 : this.Port;
                        dav.User = user;
                        dav.Pass = pass;
                        dav.BasePath = Path.GetDirectoryName(this.LocalPath).Replace('\\', '/');
                        dav.Download(Path.GetFileName(this.LocalPath), lPath);
                        autoResetEvent.WaitOne();
                        return lPath;
                    default:
                        break;
                }

                throw new ArgumentException("Unsupported scheme provided.");
            }
        }

        // TODO: add prompt handler!
        private static bool ValidateServerCertficate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

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
