using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueLib.Exceptions
{
    class AlreadyMountedException : SystemException
    {
        public AlreadyMountedException(string message) : base(message) { }
    }
}
