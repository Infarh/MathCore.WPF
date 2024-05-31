using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MathCore.WPF.Generators.Infrastructure.Extensions;

internal static class SyntaxTokenEx
{
    public static bool IsKind(this SyntaxToken token, params SyntaxKind[] kinds)
    {
        foreach (var kind in kinds)
            if (token.RawKind == (int)kind)
                return true;

        return false;
    }
}