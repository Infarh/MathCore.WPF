using System;
using System.Globalization;
using System.Windows.Data;
using MathCore.Annotations;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    public class LambdaMultiConverter: IMultiValueConverter
    {
        public delegate object? Converter(object[]? Values, Type? TargetValueType, object? Parameter, CultureInfo? Culture);

        public delegate object[]? ConverterBack(object? Value, Type[]? SourceValueTypes, object? Parameter, CultureInfo? Culture);

        private readonly Converter _Conversation;

        private readonly ConverterBack? _BackConversation;

        public LambdaMultiConverter(Func<object[]?, object?> SimpleConverter, [CanBeNull] Func<object?, object[]?>? SimpleBackConverter = null)
            : this(
                (v,_,_,_) => SimpleConverter(v), 
                SimpleBackConverter is null ? null : (ConverterBack)((v,_,_,_) => SimpleBackConverter!(v)))
        { }

        public LambdaMultiConverter(Converter Conversation,
            ConverterBack? BackConversation = null)
        {
            _Conversation = Conversation;
            _BackConversation = BackConversation;
        }

        public object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => _Conversation(vv, t, p, c);

        public object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) => (_BackConversation ?? throw new NotSupportedException("Обратное преобразование не поддерживается"))(v, tt, p, c);
    }
}