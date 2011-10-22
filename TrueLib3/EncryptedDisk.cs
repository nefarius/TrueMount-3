using System;
using System.Collections.Generic;

namespace TrueLib
{
    [Serializable()]
    public class EncryptedDisk : EncryptedMedia
    {
        /// <summary>
        /// The caption (title) of the disk.
        /// </summary>
        public string DiskCaption { get; set; }
        /// <summary>
        /// The numeric signature of the disk.
        /// </summary>
        public uint DiskSignature { get; set; }

        /// <summary>
        /// Creates a new disk and assigns caption, signature.
        /// </summary>
        /// <param name="caption">The disks caption.</param>
        /// <param name="signature">The disks signature.</param>
        public EncryptedDisk(string caption, uint signature)
            : base()
        {
            this.DiskCaption = caption;
            this.DiskSignature = signature;
        }

        public override bool IsOnline
        {
            get { return SystemDevices.IsDiskOnline(DiskCaption, DiskSignature); }
        }

        public override bool Equals(object obj)
        {
            EncryptedDisk ep = (EncryptedDisk)obj;

            if (DiskCaption == ep.DiskCaption &&
                DiskSignature == ep.DiskSignature)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.DiskCaption;
        }
    }
}
