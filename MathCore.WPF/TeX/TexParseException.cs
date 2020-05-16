using System;

namespace MathCore.WPF.TeX
{
    public class TexParseException : Exception
    {
        internal TexParseException(string message)
            : base(message)
        {
        }

        internal TexParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}