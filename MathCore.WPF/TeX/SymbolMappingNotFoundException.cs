using System;

namespace MathCore.WPF.TeX
{
    public class SymbolMappingNotFoundException : Exception
    {
        internal SymbolMappingNotFoundException(string symbolName)
            : base($"Cannot find mapping for the symbol with name '{symbolName}'.")
        {
        }
    }
}
