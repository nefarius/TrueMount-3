using System;
using System.IO;

namespace TrueLib
{
    [Serializable()]
    public class TriggerDevice : IPhysicalMedia
    {
        public Guid Guid { get; private set; }
        public bool IsActive { get; set; }

        public TriggerDevice()
        {
#if !DEBUG
            this.Guid = Guid.NewGuid();
#else
            this.Guid = new Guid("a4a48e95-92d3-4242-839b-ce4dae991346");
#endif
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(System.DBNull))
            {
                TriggerDevice td = (TriggerDevice)obj;
                return (this.Guid == td.Guid);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.Guid.ToString();
        }

        public bool IsOnline
        {
            get
            {
                foreach (var drive in DriveInfo.GetDrives())
                {
                    string path = Path.Combine(drive.RootDirectory.Name, Configuration.IdentityFile);
                    if(File.Exists(path))
                    {
                        using (StreamReader sr = new StreamReader(path))
                        {
                            if (Guid == new Guid(sr.ReadLine()))
                                return true;
                        }
                    }
                }

                return false;
            }
        }

        public void Denote(string drive)
        {
            string file = Path.Combine(drive, Configuration.IdentityFile);
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.WriteLine(Guid.ToString());
            }
            File.SetAttributes(file, File.GetAttributes(file) | 
                FileAttributes.Hidden | FileAttributes.System);
        }
    }
}
