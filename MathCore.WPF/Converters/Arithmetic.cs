using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(Arithmetic))]
#if NET7_0_OR_GREATER
public partial class Arithmetic : ValueConverter
#else
public class Arithmetic : ValueConverter
#endif
{
    private const string __ArithmeticParseExpression = @"([+\-*/]{1,1})\s{0,}(\-?[\d\.]+)";
#if NET7_0_OR_GREATER
    private readonly Regex _Pattern = MyRegex();
    [GeneratedRegex(__ArithmeticParseExpression, RegexOptions.Compiled)]
    private static partial Regex MyRegex();
#else
    private readonly Regex _Pattern = new(__ArithmeticParseExpression, RegexOptions.Compiled); 
#endif

    protected override object? Convert(object? v, Type t, object? p, CultureInfo c)
    {
        if (v is not double value || p is not string { Length: > 0 } p_str) return null;

        var pattern = _Pattern.Match(p_str);
        if (pattern.Groups.Count != 3) return null;
        var op = pattern.Groups[1].Value.Trim();
        p_str = pattern.Groups[2].Value;

        if (!double.TryParse(p_str, out var p_value)) return null;

        return op switch
        {
            "+" => (value + p_value),
            "-" => (value - p_value),
            "*" => (value * p_value),
            "/" => (value / p_value),
            _ => throw new NotSupportedException($"Операция {op} не поддерживается")
        };
    }

    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c)
    {
        if (v is not double d || p is not string { Length: > 0 } p_str) return null;

        var pattern = _Pattern.Match(p_str);
        if (pattern.Groups.Count != 3) return null;
        var op = pattern.Groups[1].Value.Trim();
        p_str = pattern.Groups[2].Value;

        return !double.TryParse(p_str, out var p_value)
            ? null
            : op switch
            {
                "+" => (d - p_value),
                "-" => (d + p_value),
                "*" => (d / p_value),
                "/" => (d * p_value),
                _ => throw new NotSupportedException($"Операция {op} не поддерживается")
            };
    }
}