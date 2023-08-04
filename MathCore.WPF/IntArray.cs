using System.Windows.Markup;

namespace MathCore.WPF;

[MarkupExtensionReturnType(typeof(int[]))]
public class IntArray(string Data) : MarkupExtension
{
    public string Data { get; set; } = Data;

    public IntArray() : this(string.Empty) { }

    public override object ProvideValue(IServiceProvider Services)
    {
        var data   = Data.AsStringPtr().Split(';', ' ');
        var values = new List<int>();
        foreach (var str in data)
            if(str.TryParseInt32() is { } value)
                values.Add(value);
        return values.ToArray();
    }
}

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

[MarkupExtensionReturnType(typeof(int[]))]
public sealed class Int32Array : MarkupExtension
{
    public int From { get; set; } = 0;
    public int To { get; set; } = 10;
    public int Step { get; set; } = 1;

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
        var result = new int[N];
        if(from < to)
            for (var i = 0; i < N; i++)
                result[i] = from + i * step;
        else
            for (var i = 0; i < N; i++)
                result[i] = from - i * step;

        return result;
    }

    #endregion
}

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