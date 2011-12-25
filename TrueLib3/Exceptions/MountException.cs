using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueLib.Exceptions
{
    public class MountException : SystemException
    {
        public MountException(string message) : base(message) { }
    }
}
