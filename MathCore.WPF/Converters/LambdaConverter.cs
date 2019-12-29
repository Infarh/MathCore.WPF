using System;
using System.Globalization;
using System.Windows.Markup;
using MathCore.Annotations;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(LambdaConverter))]
    public class LambdaConverter : ValueConverter
    {
        [NotNull] private readonly Func<object, Type, object, CultureInfo, object> _To;
        [CanBeNull] private readonly Func<object, Type, object, CultureInfo, object> _From;

        public LambdaConverter([NotNull] Func<object, Type, object, CultureInfo, object> To, [CanBeNull] Func<object, Type, object, CultureInfo, object> From = null)
        {
            _To = To;
            _From = From;
        }

        protected override object Convert(object v, Type t, object p, CultureInfo c) => _To(v, t, p, c);

        protected override object ConvertBack(object v, Type t, object p, CultureInfo c) => (_From ?? throw new NotSupportedException("Обратное преобразование не поддерживается")).Invoke(v, t, p, c);
    }
}