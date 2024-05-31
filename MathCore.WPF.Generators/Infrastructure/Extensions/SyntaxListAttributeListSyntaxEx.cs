using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MathCore.WPF.Generators.Infrastructure.Extensions;

internal static class SyntaxListAttributeListSyntaxEx
{
    public static bool ExistAttribute(this SyntaxList<AttributeListSyntax> Attributes, string AttributeName)
    {
        foreach (var attribute in Attributes.SelectMany(static a => a.Attributes))
            if (attribute is { Name: IdentifierNameSyntax { Identifier.ValueText: { Length: > 0 } name } } && name == AttributeName)
                return true;

        return false;
    }

    public static bool ContainsAttributeText(this SyntaxList<AttributeListSyntax> Attributes, string AttributeName)
    {
        foreach (var attribute in Attributes.SelectMany(static a => a.Attributes))
            if (attribute is { Name: IdentifierNameSyntax { Identifier.ValueText: { Length: > 0 } name } } && name.Contains(AttributeName))
                return true;

        return false;
    }
}