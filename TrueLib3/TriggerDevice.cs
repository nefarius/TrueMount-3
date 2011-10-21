using System;

namespace TrueLib
{
    [Serializable()]
    public class TriggerDevice : IPhysicalMedia
    {
        public string Caption { get; set; }
        public uint Signature { get; set; }
        public uint PartitionIndex { get; set; }
        public bool IsActive { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(System.DBNull))
            {
                TriggerDevice td = (TriggerDevice)obj;
                if (Caption == td.Caption &&
                    Signature == td.Signature &&
                    PartitionIndex == td.PartitionIndex)
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.Caption;
        }

        public bool IsOnline()
        {
            return SystemDevices.IsPartitionOnline(Caption, Signature, PartitionIndex);
        }
    }
}
