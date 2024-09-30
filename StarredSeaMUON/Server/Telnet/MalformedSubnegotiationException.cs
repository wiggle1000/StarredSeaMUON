using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Server.Telnet
{
    internal class MalformedSubnegotiationException : Exception
    {
        public MalformedSubnegotiationException(string? message) : base(message)
        {
        }
    }
}
