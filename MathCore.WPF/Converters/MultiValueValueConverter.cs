using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using MathCore.Annotations;

namespace MathCore.WPF.Converters
{
    /// <summary>Конвертер величин</summary>
    [MarkupExtensionReturnType(typeof(MultiValueValueConverter))]
    public abstract class MultiValueValueConverter : MarkupExtension, IMultiValueConverter
    {
        /// <inheritdoc />
        [NotNull]
        public override object ProvideValue(IServiceProvider sp) => this;

        /// <summary>Преобразование значений</summary>
        /// <param name="vv">Массив преобразуемых значений</param>
        /// <param name="t">Требуемый тип значения</param>
        /// <param name="p">Параметр преобразования</param>
        /// <param name="c">Сведения о культуре</param>
        /// <returns>Преобразованное значение</returns>
        protected abstract object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c);

        /// <summary>Обратное преобразование значения</summary>
        /// <param name="v">Значение, для которого требуется выполнить обратное преобразование</param>
        /// <param name="tt">Массив требуемых типов данных значений</param>
        /// <param name="p">Параметр преобразования</param>
        /// <param name="c">Сведения о культуре</param>
        /// <returns>Исходные значения</returns>
        /// <exception cref="NotSupportedException">Генерируется при отсутствии переопределения в классах наследниках</exception>
        protected virtual object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) => throw new NotSupportedException("Обратное преобразование не поддерживается");

        /// <inheritdoc />
        object? IMultiValueConverter.Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => Convert(vv, t, p, c);

        /// <inheritdoc />
        object[]? IMultiValueConverter.ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) => ConvertBack(v, tt, p, c);
    }
}