namespace MathCore.WPF.TeX;

public class TextStyleMappingNotFoundException : Exception
{
    internal TextStyleMappingNotFoundException(string TextStyleName)
        : base($"Cannot find mapping for the style with name '{TextStyleName}'.")
    {
    }
}