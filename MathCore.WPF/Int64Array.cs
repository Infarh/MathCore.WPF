using System.Windows.Markup;

namespace MathCore.WPF;

/// <summary>MarkupExtension для создания массива целых чисел из строки.</summary>
/// <example>
/// <code>
/// <![CDATA[
/// <x:Array xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
///          xmlns:sys="clr-namespace:System;assembly=mscorlib"
///          xmlns:local="clr-namespace:MathCore.WPF">
///     <local:IntArray Data="1; 2 3; 4"/>
/// </x:Array>
/// ]]>
/// </code>
/// </example>
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