using System.Globalization;
using System.Windows.Data;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

public class LambdaMultiConverter(LambdaMultiConverter.Converter Conversation,
LambdaMultiConverter.ConverterBack? BackConversation = null) : IMultiValueConverter
{
    public LambdaMultiConverter(Func<object[]?, object?> SimpleConverter, Func<object?, object[]?>? SimpleBackConverter = null)
        : this(
            (v, _, _, _) => SimpleConverter(v),
            SimpleBackConverter is null ? null : ((v, _, _, _) => SimpleBackConverter!(v)))
    { }

    public delegate object? Converter(object[]? Values, Type? TargetValueType, object? Parameter, CultureInfo? Culture);

    public delegate object[]? ConverterBack(object? Value, Type[]? SourceValueTypes, object? Parameter, CultureInfo? Culture);

    public object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => Conversation(vv, t, p, c);

    public object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) => (BackConversation ?? throw new NotSupportedException("Обратное преобразование не поддерживается"))(v, tt, p, c);
}