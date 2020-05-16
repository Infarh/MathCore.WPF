using System;
using System.Globalization;
using System.Text.RegularExpressions;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    public class Arithmetic : ValueConverter
    {
        private const string __ArithmeticParseExpression = "([+\\-*/]{1,1})\\s{0,}(\\-?[\\d\\.]+)";
        private readonly Regex _Pattern = new Regex(__ArithmeticParseExpression, RegexOptions.Compiled);

        [CanBeNull]
        protected override object? Convert(object? v, Type t, object? p, CultureInfo c)
        {
            if (!(v is double) || !(p is string p_str)) return null;

            if (p_str.Length == 0) return null;
            var pattern = _Pattern.Match(p_str);
            if (pattern.Groups.Count != 3) return null;
            var op = pattern.Groups[1].Value.Trim();
            p_str = pattern.Groups[2].Value;

            if (!double.TryParse(p_str, out var p_value)) return null;
            var value = (double)v;
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
            if (!(v is double) || !(p is string p_str)) return null;

            if (p_str.Length == 0) return null;
            var pattern = _Pattern.Match(p_str);
            if (pattern.Groups.Count != 3) return null;
            var op = pattern.Groups[1].Value.Trim();
            p_str = pattern.Groups[2].Value;

            if (!double.TryParse(p_str, out var p_value)) return null;
            return op switch
            {
                "+" => ((double) v - p_value),
                "-" => ((double) v + p_value),
                "*" => ((double) v / p_value),
                "/" => ((double) v * p_value),
                _ => throw new NotSupportedException($"Операция {op} не поддерживается")
            };
        }
    }
}