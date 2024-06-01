using System.Collections.Concurrent;

using MathCore.MathParser;
using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

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

    public double ParameterDefault { get; set; } = double.NaN;

    public string ArgumentName { get; set; } = "x";

    public string ParameterName { get; set; } = "p";

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

    /// <inheritdoc />
    protected override double Convert(double v, double? p = null) => _Converter is { } converter 
        ? converter(v, p ?? ParameterDefault) 
        : base.Convert(v, p);
}
