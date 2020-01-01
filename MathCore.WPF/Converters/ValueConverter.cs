using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using MathCore.Annotations;

namespace MathCore.WPF.Converters
{
    /// <summary>��������� �������</summary>
    [MarkupExtensionReturnType(typeof(ValueConverter))]
    public abstract class ValueConverter : MarkupExtension, IValueConverter
    {
        /// <inheritdoc />
        [NotNull]
        public override object ProvideValue(IServiceProvider sp) => this;

        /// <summary>�������������� ��������</summary>
        /// <param name="v">������������� ��������</param>
        /// <param name="t">��������� ��� ��������</param>
        /// <param name="p">�������� ��������������</param>
        /// <param name="c">�������� � ��������</param>
        /// <returns>��������������� ��������</returns>
        [CanBeNull]
        protected abstract object? Convert([CanBeNull] object? v, Type t, object? p, CultureInfo c);

        /// <summary>�������� �������������� ��������</summary>
        /// <param name="v">��������, ��� �������� ��������� ��������� �������� ��������������</param>
        /// <param name="t">��������� ��� ������ ��������</param>
        /// <param name="p">�������� ��������������</param>
        /// <param name="c">�������� � ��������</param>
        /// <returns>�������� ��������</returns>
        /// <exception cref="NotSupportedException">������������ ��� ���������� ��������������� � ������� �����������</exception>
        [CanBeNull]
        protected virtual object? ConvertBack([CanBeNull] object? v, Type t, object? p, CultureInfo c) => throw new NotSupportedException("�������� �������������� �� ��������������");

        /// <inheritdoc />
        object? IValueConverter.Convert(object? v, Type t, object? p, CultureInfo c) => Convert(v, t, p, c);

        /// <inheritdoc />
        object? IValueConverter.ConvertBack(object? v, Type t, object? p, CultureInfo c) => ConvertBack(v, t, p, c);
    }
}