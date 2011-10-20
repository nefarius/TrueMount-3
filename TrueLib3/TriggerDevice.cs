using System;

namespace TrueLib
{
    [Serializable()]
    class TriggerDevice
    {
        public string Caption { get; set; }
        public uint Signature { get; set; }
        public uint PartitionIndex { get; set; }
        public bool IsActive { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(System.DBNull))
            {
                TriggerDevice kd = (TriggerDevice)obj;
                if (Caption == kd.Caption &&
                    Signature == kd.Signature &&
                    PartitionIndex == kd.PartitionIndex)
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
    }
}
