using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Универсальный Lambda конвертер, использующий делегаты для преобразования</summary>
[MarkupExtensionReturnType(typeof(LambdaConverter))]
public class LambdaConverter(LambdaConverter.Converter To, LambdaConverter.ConverterBack? From = null) : ValueConverter
{
    /// <summary>Делегат преобразования</summary>
    public delegate object? Converter(object? Value, Type? TargetValueType, object? Parameter, CultureInfo? Culture);

    /// <summary>Делегат обратного преобразования</summary>
    public delegate object? ConverterBack(object? Value, Type? SourceValueType, object? Parameter, CultureInfo? Culture);

    /// <inheritdoc />
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => To(v, t, p, c);

    /// <summary>Выполняет обратное преобразование, если доступен делегат</summary>
    /// <exception cref="NotSupportedException">Если обратное преобразование не поддерживается</exception>
    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => (From ?? throw new NotSupportedException("Обратное преобразование не поддерживается")).Invoke(v, t, p, c);
}