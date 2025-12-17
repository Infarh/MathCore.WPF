using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Проверяет, что все последующие значения не больше первого</summary>
[MarkupExtensionReturnType(typeof(GreaterThanMulti))]
public class GreaterOrEqualThanMulti : MultiValueValueConverter
{
    /// <summary>
    /// Преобразует массив объектов в булево значение, указывающее
    /// на то, что все последующие значения не больше первого.
    /// </summary>
    /// <param name="vv">Массив значений для проверки.</param>
    /// <param name="t">Тип, в который требуется преобразовать значения.</param>
    /// <param name="p">Дополнительный параметр, который может быть использован при преобразовании.</param>
    /// <param name="c">Культура, которая может быть использована при преобразовании.</param>
    /// <returns>
    /// Возвращает <see cref="true"/>, если все последующие значения не больше первого;
    /// иначе - <see cref="false"/>. Если входные данные недопустимы,
    /// возвращает <see cref="Binding.DoNothing"/>.
    /// </returns>
    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c)
    {
        if (vv is not { Length: > 1 })
            return Binding.DoNothing;

        if (!DoubleValueConverter.TryConvertToDouble(vv[0], c, out var first_value))
            return Binding.DoNothing;

        for (var i = 1; i < vv.Length; i++)
        {
            if (!DoubleValueConverter.TryConvertToDouble(vv[i], c, out var value))
                return Binding.DoNothing;
            if (value > first_value)
                return false;
        }

        return true;
    }
}