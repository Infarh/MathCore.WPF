using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using MathCore.Annotations;

namespace MathCore.WPF.Converters
{
    /// <summary>Конвертер величин</summary>
    [MarkupExtensionReturnType(typeof(ValueConverter))]
    public abstract class ValueConverter : MarkupExtension, IValueConverter
    {
        /// <inheritdoc />
        [NotNull]
        public override object ProvideValue(IServiceProvider sp) => this;

        /// <summary>Преобразование значения</summary>
        /// <param name="v">Преобразуемое значение</param>
        /// <param name="t">Требуемый тип значения</param>
        /// <param name="p">Параметр преобразования</param>
        /// <param name="c">Сведения о культуре</param>
        /// <returns>Преобразованное значение</returns>
        [CanBeNull]
        protected abstract object? Convert([CanBeNull] object? v, Type t, object? p, CultureInfo c);

        /// <summary>Обратное преобразование значения</summary>
        /// <param name="v">Значение, для которого требуется выполнить обратное преобразование</param>
        /// <param name="t">Требуемый тип данных значения</param>
        /// <param name="p">Параметр преобразования</param>
        /// <param name="c">Сведения о культуре</param>
        /// <returns>Исходное значение</returns>
        /// <exception cref="NotSupportedException">Генерируется при отсутствии переопределения в классах наследниках</exception>
        [CanBeNull]
        protected virtual object? ConvertBack([CanBeNull] object? v, Type t, object? p, CultureInfo c) => throw new NotSupportedException("Обратное преобразование не поддерживается");

        /// <inheritdoc />
        object? IValueConverter.Convert(object? v, Type t, object? p, CultureInfo c) => Convert(v, t, p, c);

        /// <inheritdoc />
        object? IValueConverter.ConvertBack(object? v, Type t, object? p, CultureInfo c) => ConvertBack(v, t, p, c);
    }
}