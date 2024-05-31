using Microsoft.CodeAnalysis;

namespace MathCore.WPF.Generators.Infrastructure.Extensions;

internal static class SymbolEx
{
    public static AttributeData? GetAttribute(this ISymbol? symbol, string AttributeName)
    {
        if (symbol?.GetAttributes() is not { IsDefaultOrEmpty: false } attributes) return null;

        foreach (var attribute in attributes)
            if (attribute is { AttributeClass.Name: { } name } && name == AttributeName)
                return attribute;

        return null;
    }

    public static AttributeData? GetAttributeLike(this ISymbol? symbol, string AttributeName)
    {
        if (symbol?.GetAttributes() is not { IsDefaultOrEmpty: false } attributes) return null;

        foreach (var attribute in attributes)
            if (attribute is { AttributeClass.Name: { } name } && name.Contains(AttributeName))
                return attribute;

        return null;
    }

    public static bool HasAttributeLike(this ISymbol? symbol, string AttributeName)
    {
        if (symbol?.GetAttributes() is not { IsDefaultOrEmpty: false } attributes) return false;

        foreach (var attribute in attributes)
            if (attribute is { AttributeClass.Name: { } name } && name.Contains(AttributeName))
                return true;

        return false;
    }
}