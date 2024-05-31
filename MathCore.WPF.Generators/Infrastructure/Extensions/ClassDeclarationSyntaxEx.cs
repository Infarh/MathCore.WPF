using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MathCore.WPF.Generators.Infrastructure.Extensions;

internal static class ClassDeclarationSyntaxEx
{
    public static SyntaxToken GetModifier(this ClassDeclarationSyntax Class, SyntaxKind kind) => Class.Modifiers.FirstOrDefault(m => m.IsKind(kind));

    public static IEnumerable<SyntaxToken> GetModifiers(this ClassDeclarationSyntax Class, SyntaxKind kind) => Class.Modifiers.Where(m => m.IsKind(kind));

    public static bool IsPartial(this ClassDeclarationSyntax Class) => Class.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

    public static SyntaxToken? GetPartialModifier(this ClassDeclarationSyntax Class) => Class.Modifiers.FirstOrDefault(m => m.IsKind(SyntaxKind.PartialKeyword));

    public static bool IsStatic(this ClassDeclarationSyntax Class) => Class.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));

    public static SyntaxToken? GetStaticModifier(this ClassDeclarationSyntax Class) => Class.Modifiers.FirstOrDefault(m => m.IsKind(SyntaxKind.StaticKeyword));

    public static IEnumerable<string> EnumAccessModifiers(this ClassDeclarationSyntax Class) => Class.Modifiers
        .Where(static m => m.IsKind(SyntaxKind.PublicKeyword, SyntaxKind.PrivateKeyword, SyntaxKind.InternalKeyword))
        .Select(static m => m.ValueText);

    public static BaseNamespaceDeclarationSyntax? GetNamespace(this ClassDeclarationSyntax Class)
    {
        SyntaxNode? node = Class;
        while (node is not null and not BaseNamespaceDeclarationSyntax)
            node = node.Parent;

        return node as BaseNamespaceDeclarationSyntax;
    }
}