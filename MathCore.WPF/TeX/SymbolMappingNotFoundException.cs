namespace MathCore.WPF.TeX;

public class SymbolMappingNotFoundException : Exception
{
    internal SymbolMappingNotFoundException(string SymbolName)
        : base($"Cannot find mapping for the symbol with name '{SymbolName}'.")
    {
    }
}