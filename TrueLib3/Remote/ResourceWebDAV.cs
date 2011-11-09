using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using net.kvdb.webdav;
using System.IO;

namespace TrueLib.Remote
{
    public class ResourceWebDAV : ResourceHTTP
    {
        public override string CachedFileName
        {
            get
            {
                AutoResetEvent autoResetEvent = new AutoResetEvent(false);
                DownloadCompleteDel dav_ListComplete = delegate
                {
                    autoResetEvent.Set();
                };
                WebDAVClient dav = new WebDAVClient();
                dav.DownloadComplete += new DownloadCompleteDel(dav_ListComplete);
                dav.Server = string.Format("{0}://{1}", Protocol, Hostname);
                dav.Port = new Nullable<int>((int)Port);
                dav.User = Username;
                dav.Pass = Password;
                dav.BasePath = Path.GetDirectoryName(this.LocalPath).Replace('\\', '/');
                dav.Download(Path.GetFileName(this.LocalPath), LocalPath);
                autoResetEvent.WaitOne();
                return LocalPath;
            }
        }

        public ResourceWebDAV(bool useHttps = true)
        {
            if (useHttps)
            {
                Protocol = "https";
                Port = 443;
            }
            else
            {
                Protocol = "http";
                Port = 80;
            }
        }
    }
}
