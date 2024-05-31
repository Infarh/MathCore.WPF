using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MathCore.WPF.Generators.Infrastructure.Extensions;

internal static class NodeSyntaxEx
{
    public static bool IsClass(this SyntaxNode node) => node is ClassDeclarationSyntax;

    public static bool IsClassWithAnyProperty(this SyntaxNode node, Func<PropertyDeclarationSyntax, bool> PropertySelector) => 
        node is ClassDeclarationSyntax { Members: var members } && 
        members.OfType<PropertyDeclarationSyntax>().Any(PropertySelector);
}