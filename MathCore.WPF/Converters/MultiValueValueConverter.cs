using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using MathCore.Annotations;

namespace MathCore.WPF.Converters
{
    /// <summary>��������� �������</summary>
    [MarkupExtensionReturnType(typeof(MultiValueValueConverter))]
    public abstract class MultiValueValueConverter : MarkupExtension, IMultiValueConverter
    {
        /// <inheritdoc />
        [NotNull]
        public override object ProvideValue(IServiceProvider sp) => this;

        /// <summary>�������������� ��������</summary>
        /// <param name="vv">������ ������������� ��������</param>
        /// <param name="t">��������� ��� ��������</param>
        /// <param name="p">�������� ��������������</param>
        /// <param name="c">�������� � ��������</param>
        /// <returns>��������������� ��������</returns>
        protected abstract object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c);

        /// <summary>�������� �������������� ��������</summary>
        /// <param name="v">��������, ��� �������� ��������� ��������� �������� ��������������</param>
        /// <param name="tt">������ ��������� ����� ������ ��������</param>
        /// <param name="p">�������� ��������������</param>
        /// <param name="c">�������� � ��������</param>
        /// <returns>�������� ��������</returns>
        /// <exception cref="NotSupportedException">������������ ��� ���������� ��������������� � ������� �����������</exception>
        protected virtual object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) => throw new NotSupportedException("�������� �������������� �� ��������������");

        /// <inheritdoc />
        object? IMultiValueConverter.Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => Convert(vv, t, p, c);

        /// <inheritdoc />
        object[]? IMultiValueConverter.ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) => ConvertBack(v, tt, p, c);
    }
}