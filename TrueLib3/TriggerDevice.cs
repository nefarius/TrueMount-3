using System;
using System.IO;

namespace TrueLib
{
    [Serializable()]
    public class TriggerDevice : IPhysicalMedia
    {
        private Guid _Guid = Guid.Empty;
        public Guid Guid
        {
            get { return _Guid; }
        }
        public bool IsActive { get; set; }

        public TriggerDevice()
        {
#if !DEBUG
            this._Guid = Guid.NewGuid();
#else
            this._Guid = new Guid("a4a48e95-92d3-4242-839b-ce4dae991346");
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
                    string path = string.Format("{0}tm3identity", drive.RootDirectory);
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
    }
}
