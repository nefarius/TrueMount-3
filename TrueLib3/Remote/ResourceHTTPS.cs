﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueLib.Remote
{
    public class ResourceHTTPS : ResourceHTTP
    {
        public ResourceHTTPS()
        {
            // Surprise, different sheme and port =)
            Protocol = "https";
            Port = 443;
        }
    }
}