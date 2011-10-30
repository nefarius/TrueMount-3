using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueLib.Exceptions
{
    class CIFSException : SystemException
    {
        public CIFSException(string message) : base(message) { }
    }
}
