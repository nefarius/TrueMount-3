using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueLib.Exceptions
{
    class StaticPasswordException : SystemException
    {
        public StaticPasswordException(string message) : base(message) { }
    }
}
