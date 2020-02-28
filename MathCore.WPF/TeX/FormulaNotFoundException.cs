using System;

namespace MathCore.WPF.TeX
{
    public class FormulaNotFoundException : Exception
    {
        internal FormulaNotFoundException(string formulaName)
            : base($"Cannot find predefined formula with name '{formulaName}'.")
        {
        }
    }
}