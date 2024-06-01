using System.Windows.Markup;

namespace MathCore.WPF;

[MarkupExtensionReturnType(typeof(short[]))]
public sealed class Int16Array : MarkupExtension
{
    public short From { get; set; } = 0;
    public short To { get; set; } = 10;

    public short Step { get; set; } = 1;

    #region Overrides of MarkupExtension

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider Services)
    {
        var from = From;
        var to   = To;
        var step = Math.Max(Math.Abs(Step), (short)1);

        if(from == to) return new[] { from };

        var N = Math.Max(from, to) - Math.Min(from, to) + 1;
        N /= step;
        var result = new short[N];
        if(from < to)
            for (var i = 0; i < N; i++)
                result[i] = (short)(from + i * step);
        else
            for (var i = 0; i < N; i++)
                result[i] = (short)(from - i * step);

        return result;
    }

    #endregion
}