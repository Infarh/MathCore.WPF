using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MathCore.WPF.Generators.Infrastructure.Extensions;

internal static class PropertyDeclarationSyntaxEx
{
    public static bool ExistAttribute(this PropertyDeclarationSyntax property, string AttributeName) => property.AttributeLists.ExistAttribute(AttributeName);

    public static bool ContainsAttributeText(this PropertyDeclarationSyntax property, string AttributeName) => property.AttributeLists.ContainsAttributeText(AttributeName);

    public static bool IsNotifyPropertyProperty(this PropertyDeclarationSyntax property) => property.ContainsAttributeText("NotifyProperty");

    public static bool IsStatic(this PropertyDeclarationSyntax property) => property.Modifiers.Any(static m => m.IsKind(SyntaxKind.StaticKeyword));

    public static bool IsDependency(this PropertyDeclarationSyntax property) => !property.IsStatic() && property.ContainsAttributeText(nameof(DependencyPropertyAttribute).TrimEnd("Attribute"));
}