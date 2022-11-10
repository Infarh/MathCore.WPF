namespace MathCore.WPF.TeX;

public class SymbolNotFoundException : Exception
{
    internal SymbolNotFoundException(string SymbolName)
        : base($"Cannot find symbol with the name '{SymbolName}'.")
    {
    }
}