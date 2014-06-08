using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtremeMirror;
using TrueLib.Exceptions;
using System.IO;

namespace TrueLib.Remote
{
    public class ResourceCIFS : RemoteResource
    {
        /// <summary>
        /// Local copy of the CIFS-Resource.
        /// </summary>
        public override string CachedFileName
        {
            get
            {
                // Build a valid UNC path (e.g. \\SERVER01\DIRECTORY\FILE.TXT)
                string uncPath = string.Format(@"\\{0}{1}", Hostname, RemotePath.Replace('/', '\\'));
                // Default return value is null on fail
                string retVal = null;
                // We need username AND password to log in with credentials.
                if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                {
                    retVal = PinvokeWindowsNetworking.connectToRemote(uncPath, Username, Password);
                    if (retVal != null)
                    {
                        throw new CIFSException(retVal);
                    }
                }
                else
                {
                    retVal = PinvokeWindowsNetworking.connectToRemote(uncPath, null, null, true);
                    if (retVal != null)
                    {
                        throw new CIFSException(retVal);
                    }
                }

                // Copy the remote file to the local directory
                File.Copy(Path.Combine(uncPath, Path.GetFileName(RemotePath)), LocalPath, true);
                // Disconnect remote destination.
                PinvokeWindowsNetworking.disconnectRemote(uncPath);
                return LocalPath;
            }
        }

        public ResourceCIFS()
        {
            // Surprise, different scheme and port =)
            Protocol = "cifs";
            Port = 445;
        }
    }
}
