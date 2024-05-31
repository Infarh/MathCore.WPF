using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace MathCore.WPF.Generators.Infrastructure.Extensions;

internal static class FieldDeclarationSyntaxEx
{
    public static bool ExistAttribute(this FieldDeclarationSyntax field, string AttributeName) => field.AttributeLists.ExistAttribute(AttributeName);

    public static bool ContainsAttributeText(this FieldDeclarationSyntax field, string AttributeName) => field.AttributeLists.ContainsAttributeText(AttributeName);

    public static bool IsNotifyPropertyField(this FieldDeclarationSyntax field) => field.ContainsAttributeText("NotifyProperty");

    public static bool IsStatic(this FieldDeclarationSyntax field) => field.Modifiers.Any(static m => m.IsKind(SyntaxKind.StaticKeyword));

    public static bool IsReadonly(this FieldDeclarationSyntax field) => field.Modifiers.Any(static m => m.IsKind(SyntaxKind.ReadOnlyKeyword));
}