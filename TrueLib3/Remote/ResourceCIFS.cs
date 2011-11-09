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
        public override string CachedFileName
        {
            get
            {
                string uncPath = string.Format(@"\\{0}{1}", Hostname, RemotePath.Replace('/', '\\'));
                string retVal = null;
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
                File.Copy(Path.Combine(uncPath, Path.GetFileName(RemotePath)), LocalPath, true);
                PinvokeWindowsNetworking.disconnectRemote(uncPath);
                return LocalPath;
            }
        }
    }
}
