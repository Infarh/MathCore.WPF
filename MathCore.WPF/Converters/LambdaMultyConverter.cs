using System;
using System.Globalization;
using System.Windows.Data;

namespace MathCore.WPF.Converters
{
    public class LambdaMultyConverter: IMultiValueConverter
    {
        private readonly Func<object[], Type, object, CultureInfo, object> _Conversation;
        private readonly Func<object, Type[], object, CultureInfo, object[]> _BackConversation;

        public LambdaMultyConverter(Func<object[], object> SimpleConverter, Func<object, object[]> SimpleBackConverter = null)
            : this((v,t,p,c) => SimpleConverter(v), 
                SimpleBackConverter == null ? (Func<object, Type[], object, CultureInfo, object[]>)null : (v,t,p,c) => SimpleBackConverter(v))
        { }

        public LambdaMultyConverter(Func<object[], Type, object, CultureInfo, object> Conversation,
            Func<object, Type[], object, CultureInfo, object[]> BackConversation = null)
        {
            _Conversation = Conversation;
            _BackConversation = BackConversation;
        }

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture) => _Conversation(value, targetType, parameter, culture);

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            if(_BackConversation != null)
                return _BackConversation(value, targetType, parameter, culture);
            else
                throw new NotSupportedException();
        }
    }
}