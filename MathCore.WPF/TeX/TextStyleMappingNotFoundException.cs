using System;

namespace MathCore.WPF.TeX
{
    public class TextStyleMappingNotFoundException : Exception
    {
        internal TextStyleMappingNotFoundException(string textStyleName)
            : base($"Cannot find mapping for the style with name '{textStyleName}'.")
        {
        }
    }
}
