namespace MathCore.WPF.TeX;

public class FormulaNotFoundException : Exception
{
    internal FormulaNotFoundException(string FormulaName)
        : base($"Cannot find predefined formula with name '{FormulaName}'.")
    {
    }
}