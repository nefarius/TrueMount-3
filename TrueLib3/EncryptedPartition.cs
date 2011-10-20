using System;
using System.Collections.Generic;

namespace TrueLib
{
    [Serializable()]
    public class EncryptedPartition : EncryptedDisk
    {
        /// <summary>
        /// The partition number.
        /// </summary>
        public uint PartitionIndex { get; set; }

        public EncryptedPartition(string caption, uint signature, uint partitionNr)
            : base(caption, signature)
        {
            this.PartitionIndex = partitionNr;
        }

        public override bool Equals(object obj)
        {
            if (base.Equals((EncryptedDisk)obj))
                if (PartitionIndex == ((EncryptedPartition)obj).PartitionIndex)
                    return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}, Partition {1}", 
                base.ToString(), 
                this.PartitionIndex.ToString());
        }
    }
}
