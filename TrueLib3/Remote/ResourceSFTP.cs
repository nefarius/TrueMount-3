using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueLib.Exceptions;
using System.Diagnostics;
using System.IO;

namespace TrueLib.Remote
{
    public class ResourceSFTP : RemoteResource
    {
        public string HostFingerprint { get; set; }
        public override string CachedFileName
        {
            get
            {
                if (string.IsNullOrEmpty(this.HostFingerprint))
                {
                    throw new WinSCPException("Host fingerprint not set.");
                }

                Process winscp = new Process();
                winscp.StartInfo.FileName = "WinSCP/winscp.com";
                winscp.StartInfo.UseShellExecute = false;
                winscp.StartInfo.CreateNoWindow = true;
                winscp.StartInfo.RedirectStandardInput = true;
                winscp.StartInfo.RedirectStandardOutput = true;
                winscp.Start();

                winscp.StandardInput.WriteLine("option batch abort");
                winscp.StandardInput.WriteLine("option confirm off");
                // establish connection to server
                string open = string.Format("open -hostkey=\"{2}\" {0}@{1}", 
                    Username, Hostname, HostFingerprint);
                winscp.StandardInput.WriteLine(open);
                // send password
                winscp.StandardInput.WriteLine(Password);
                // change to desired directory on server
                string cd = string.Format("cd {0}", Path.GetDirectoryName(RemotePath).Replace('\\', '/'));
                winscp.StandardInput.WriteLine(cd);
                // set transfer mode
                winscp.StandardInput.WriteLine("option transfer binary");
                // download the file to local directory
                string get = string.Format("get \"{0}\" \"{1}\"", Path.GetFileName(RemotePath), LocalPath);
                winscp.StandardInput.WriteLine(get);
                // close the session
                winscp.StandardInput.WriteLine("close");
                // exit WinSCP
                winscp.StandardInput.WriteLine("exit");

                // close input stream
                winscp.StandardInput.Close();

                // Wait for process to completely shut down
                winscp.WaitForExit();

                if (!File.Exists(LocalPath))
                {
                    throw new WinSCPException(winscp.StandardOutput.ReadToEnd());
                }

                return LocalPath;
            }
        }
    }
}
