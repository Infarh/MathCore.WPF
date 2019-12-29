using System;

namespace MathCore.WPF.TeX
{
    public class SymbolNotFoundException : Exception
    {
        internal SymbolNotFoundException(string symbolName)
            : base($"Cannot find symbol with the name '{symbolName}'.")
        {
        }
    }
}
