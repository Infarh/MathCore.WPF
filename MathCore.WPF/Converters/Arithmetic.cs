using System;
using System.Text.RegularExpressions;

namespace MathCore.WPF.Converters
{
    public class Arithmetic : ValueConverter
    {
        private const string c_ArithmeticParseExpression = "([+\\-*/]{1,1})\\s{0,}(\\-?[\\d\\.]+)";
        private readonly Regex _Pattern = new Regex(c_ArithmeticParseExpression, RegexOptions.Compiled);

        protected override object Convert(object v, Type t, object p, System.Globalization.CultureInfo c)
        {
            if (!(v is double) || p == null) return null;
            var p_str = p.ToString();

            if (p_str.Length <= 0) return null;
            var pattern = _Pattern.Match(p_str);
            if (pattern.Groups.Count != 3) return null;
            var op = pattern.Groups[1].Value.Trim();
            p_str = pattern.Groups[2].Value;

            if (!double.TryParse(p_str, out var p_value)) return null;
            var value = (double)v;
            switch (op)
            {
                case "+": return value + p_value;
                case "-": return value - p_value;
                case "*": return value * p_value;
                case "/": return value / p_value;
                default: throw new NotSupportedException($"Операция {op} не поддерживается");
            }
        }

        protected override object ConvertBack(object v, Type t, object p, System.Globalization.CultureInfo c)
        {
            if (!(v is double) || p == null) return null;
            var p_str = p.ToString();

            if (p_str.Length <= 0) return null;
            var pattern = _Pattern.Match(p_str);
            if (pattern.Groups.Count != 3) return null;
            var op = pattern.Groups[1].Value.Trim();
            p_str = pattern.Groups[2].Value;

            if (!double.TryParse(p_str, out var p_value)) return null;
            switch (op)
            {
                case "+": return (double)v - p_value;
                case "-": return (double)v + p_value;
                case "*": return (double)v / p_value;
                case "/": return (double)v * p_value;
                default: throw new NotSupportedException($"Операция {op} не поддерживается");
            }
        }
    }
}
