using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using AlexPilotti.FTPS.Client;
using AlexPilotti.FTPS.Common;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Diagnostics;

namespace TrueLib
{
    static class RemoteResource
    {
        public static string FetchResource(Uri uri, string hostFingerprint = null)
        {
            string lPath = Path.Combine(Configuration.TempPath, Path.GetFileName(uri.LocalPath));
            string user = uri.UserInfo.Split(':')[0];
            string pass = uri.UserInfo.Split(':')[1];

            switch ((Schemes)Enum.Parse(typeof(Schemes), uri.Scheme, true))
            {
                case Schemes.File:
                    return uri.LocalPath;
                case Schemes.HTTP:
                case Schemes.HTTPS:
                case Schemes.FTP:
                    using (WebClient web = new WebClient())
                    {
                        web.DownloadFile(uri, lPath);
                    }
                    return lPath;
                case Schemes.FTPeS:
                    ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertficate;
                    using (FTPSClient ftps = new FTPSClient())
                    {
                        NetworkCredential login = new NetworkCredential(user, pass);
                        ftps.Connect(uri.Host,
                            login,
                            ESSLSupportMode.CredentialsRequired | ESSLSupportMode.DataChannelRequested,
                            new RemoteCertificateValidationCallback(ValidateServerCertficate));
                        ftps.GetFile(uri.LocalPath, lPath);
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
                    string open = string.Format("open -hostkey=\"{2}\" {0}@{1}", user, uri.Host, hostFingerprint);
                    winscp.StandardInput.WriteLine(open);
                    // send password
                    winscp.StandardInput.WriteLine(pass);
                    // change to desired directory on server
                    string cd = string.Format("cd {0}", Path.GetDirectoryName(uri.LocalPath).Replace('\\', '/'));
                    winscp.StandardInput.WriteLine(cd);
                    // set transfer mode
                    winscp.StandardInput.WriteLine("option transfer binary");
                    // download the file to local directory
                    string get = string.Format("get \"{0}\" \"{1}\"", Path.GetFileName(uri.LocalPath), lPath);
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

        // TODO: add prompt handler!
        private static bool ValidateServerCertficate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
