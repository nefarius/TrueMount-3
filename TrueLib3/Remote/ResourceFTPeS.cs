using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using AlexPilotti.FTPS.Client;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace TrueLib.Remote
{
    public class ResourceFTPeS : RemoteResource
    {
        public override string CachedFileName
        {
            get
            {
                ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertficate;
                using (FTPSClient ftps = new FTPSClient())
                {
                    NetworkCredential login = new NetworkCredential(Username, Password);
                    ftps.Connect(Hostname, 
                        login,
                        ESSLSupportMode.CredentialsRequired | ESSLSupportMode.DataChannelRequested,
                        new RemoteCertificateValidationCallback(ValidateServerCertficate));
                    ftps.GetFile(RemotePath, LocalPath);
                }
                return LocalPath;
            }
        }

        // TODO: add prompt handler!
        private static bool ValidateServerCertficate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
