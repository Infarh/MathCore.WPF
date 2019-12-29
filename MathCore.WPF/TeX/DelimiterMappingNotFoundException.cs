using System;

namespace MathCore.WPF.TeX
{
    public class DelimiterMappingNotFoundException : Exception
    {
        internal DelimiterMappingNotFoundException(char delimiter)
            : base($"Cannot find delimeter mapping for the character '{delimiter}'.")
        {
        }
    }
}
