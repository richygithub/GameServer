using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UseLibuv
{
    public sealed class InvalidBufferException : IOException
    {
        internal InvalidBufferException(string message)
            : base(message)
        {
        }

        internal InvalidBufferException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal static InvalidBufferException MalformedString()
        {
            return new InvalidBufferException(
                "InputStream encountered a malformed string.");

        }
    }
}
