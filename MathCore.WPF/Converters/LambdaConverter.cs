using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(LambdaConverter))]
public class LambdaConverter(LambdaConverter.Converter To, LambdaConverter.ConverterBack? From = null) : ValueConverter
{
    public delegate object? Converter(object? Value, Type? TargetValueType, object? Parameter, CultureInfo? Culture);

    public delegate object? ConverterBack(object? Value, Type? SourceValueType, object? Parameter, CultureInfo? Culture);

    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => To(v, t, p, c);

    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => (From ?? throw new NotSupportedException("Обратное преобразование не поддерживается")).Invoke(v, t, p, c);
}