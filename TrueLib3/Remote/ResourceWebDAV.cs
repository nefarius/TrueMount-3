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
                // The default call is async, but we want a sync call
                AutoResetEvent autoResetEvent = new AutoResetEvent(false);
                // Delegate to fire the event on completion
                DownloadCompleteDel dav_ListComplete = delegate
                {
                    autoResetEvent.Set();
                };
                WebDAVClient dav = new WebDAVClient();
                // Set the event
                dav.DownloadComplete += new DownloadCompleteDel(dav_ListComplete);
                // Build server URI
                dav.Server = string.Format("{0}://{1}", Protocol, Hostname);
                // Set port (orly?)
                dav.Port = new Nullable<int>((int)Port);
                // ...
                dav.User = Username;
                // *sigh*
                dav.Pass = Password;
                // DIrectory of the remote file...
                dav.BasePath = Path.GetDirectoryName(RemotePath).Replace('\\', '/');
                // GET IT!
                dav.Download(Path.GetFileName(RemotePath), LocalPath);
                // Finished! =)
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
