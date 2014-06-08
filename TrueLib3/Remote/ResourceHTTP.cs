using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace TrueLib.Remote
{
    public class ResourceHTTP : RemoteResource
    {
        public override string CachedFileName
        {
            get
            {
                Uri url = null;
                using (WebClient web = new WebClient())
                {
                    if(!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                    {
                        // Build URI with authentication data
                        url = new Uri(string.Format("{0}://{1}:{2}@{3}{4}/{5}",
                            Protocol.ToString().ToLower(), Username, Password,
                            Hostname, Port, RemotePath));
                    }
                    else
                    {
                        // Build URI without authentication data
                        url = new Uri(string.Format("{0}://{1}:{2}/{3}",
                            Protocol.ToString().ToLower(), Hostname, Port, RemotePath));
                    }

                    // Fetch the file
                    web.DownloadFile(url, LocalPath);
                }
                return LocalPath;
            }
        }

        public ResourceHTTP()
        {
            Protocol = "http";
            Port = 80;
        }
    }
}
