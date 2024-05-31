namespace MathCore.WPF.Generators.Infrastructure.Extensions;

internal static class NamedTypeSymbolEx
{
    public static IEnumerable<IPropertySymbol> EnumNonStaticProperties(this INamedTypeSymbol symbol) =>
        symbol
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => !p.IsStatic);
}
