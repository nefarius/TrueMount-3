using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueLib.Exceptions
{
    class WinSCPException : SystemException
    {
        public WinSCPException(string message) : base(message) { }
    }
}
