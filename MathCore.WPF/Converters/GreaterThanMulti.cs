using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Проверяет, что все последующие значения меньше первого</summary>
[MarkupExtensionReturnType(typeof(GreaterThanMulti))]
public class GreaterThanMulti : MultiValueValueConverter
{
    /// <summary>Проверяет, что все последующие значения меньше первого</summary>
    /// <param name="vv">Массив значений, которые требуется проверить</param>
    /// <param name="t">Тип, в который требуется произвести преобразование</param>
    /// <param name="p">Дополнительные параметры преобразования</param>
    /// <param name="c">Культура, используемая при преобразовании</param>
    /// <returns>
    /// Возвращает <see langword="true"/>, если все последующие значения меньше первого;
    /// возвращает <see langword="false"/>, если хотя бы одно значение больше или равно первому;
    /// возвращает <see cref="Binding.DoNothing"/>, если входные данные некорректны.
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
            if (value >= first_value)
                return false;
        }

        return true;
    }
}