using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueLib.Exceptions
{
    class TrueCryptException : SystemException
    {
        public TrueCryptException(string message) : base(message) { }
    }
}
