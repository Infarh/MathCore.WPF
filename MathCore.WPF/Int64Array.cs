using System.Windows.Markup;

namespace MathCore.WPF;

[MarkupExtensionReturnType(typeof(long[]))]
public sealed class Int64Array : MarkupExtension
{
    public long From { get; set; } = 0;
    public long To { get; set; } = 10;
    public long Step { get; set; } = 1;

    #region Overrides of MarkupExtension

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider Services)
    {
        var from = From;
        var to   = To;
        var step = Math.Max(Math.Abs(Step), 1);
        if(from == to) return new[] { From };

        var N = Math.Max(from, to) - Math.Min(from, to) + 1;
        N /= step;
        var result = new long[N];
        if (from < to)
            for (var i = 0; i < N; i++)
                result[i] = from + i * step;
        else
            for (var i = 0; i < N; i++)
                result[i] = from - i * step;

        return result;
    }

    #endregion
}