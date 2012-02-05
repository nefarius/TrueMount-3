using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueLib
{
    [Serializable()]
    public class MountOptions
    {
        public bool Readonly { get; set; }
        public bool Removable { get; set; }
        public bool Timestamp { get; set; }
        public bool System { get; set; }
        public bool Headerbak { get; set; }
        public bool Recovery { get; set; }

        /// <summary>
        /// Returns a ready-to-use argument line string.
        /// </summary>
        public override string ToString()
        {
            StringBuilder mOpts = new StringBuilder();
            if (Readonly)
                mOpts.Append("/m ro ");
            if (Removable)
                mOpts.Append("/m rm ");
            if (Timestamp)
                mOpts.Append("/m ts ");
            if (System)
                mOpts.Append("/m sm ");
            if (Headerbak)
                mOpts.Append("/m bk ");
            if (Recovery)
                mOpts.Append("/m recovery ");

            return mOpts.ToString().Trim();
        }
    }
}
