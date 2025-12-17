using System.Collections.Concurrent;
using System.Globalization;

using MathCore.MathParser;
using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Вычисляет значение выражения, скомпилированного в делегат</summary>
public class Expr : SimpleDoubleValueConverter
{
    private static readonly ConcurrentDictionary<(string Expr, string ArgName, string ParameterName), Func<double, double, double>> __Converters = new();

    private static Func<double, double, double> GetConverter((string Expr, string ArgName, string ParameterName) value)
    {
        var parser = new ExpressionParser();

        var expr = parser.Parse(value.Expr);
        var result = (Func<double, double, double>)expr.Compile(value.ArgName, value.ParameterName);

        return result;
    }

    private string _Expression;

    private Func<double, double, double>? _Converter;

    /// <summary>Значение параметра по умолчанию</summary>
    public double ParameterDefault { get; set; } = double.NaN;

    /// <summary>Имя аргумента в выражении</summary>
    public string ArgumentName { get; set; } = "x";

    /// <summary>Имя параметра в выражении</summary>
    public string ParameterName { get; set; } = "p";

    /// <summary>Строковое выражение для компиляции</summary>
    public string Expression
    {
        get => _Expression;
        set
        {
            if(Equals(_Expression, value)) return;
            _Expression = value;

            _Converter = __Converters.GetOrAdd((value, ArgumentName, ParameterName), GetConverter);
        }
    }

    public Expr() { }

    public Expr(string Expression) => this.Expression = Expression;

    /// <summary>Преобразует входное значение с использованием скомпилированного выражения или вызывает базовое поведение при отсутствии выражения</summary>
    protected override double Convert(double v, double? p = null) => _Converter is { } converter
        ? converter(v, p ?? ParameterDefault)
        : base.Convert(v, p);
}
