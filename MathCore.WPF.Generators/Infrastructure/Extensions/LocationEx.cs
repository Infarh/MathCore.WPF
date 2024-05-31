using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MathCore.WPF.Generators.Infrastructure.Extensions;

internal static class LocationEx
{
    public static Location Union(this Location Start, Location End)
    {
        if (Start is not
            {
                SourceTree: { } start_tree,
                SourceSpan:
                {
                    Start: var start_start,
                    End: var start_end
                }
            }) throw new InvalidOperationException("Стартовое положение не содержит ссылки на синтаксическое дерево");
        if (End is not
            {
                SourceTree: { } end_tree,
                SourceSpan:
                {
                    Start: var end_start,
                    End: var end_end
                }
            }) throw new InvalidOperationException("Конечное положение не содержит ссылки на синтаксическое дерево");
        if (start_tree != end_tree)
            throw new InvalidOperationException("Синтаксические деревья начального и конечного положения не совпадают.");

        return Location.Create(start_tree,
            TextSpan.FromBounds(
                Math.Min(start_start, end_start),
                Math.Max(start_end, end_end)));
    }
}