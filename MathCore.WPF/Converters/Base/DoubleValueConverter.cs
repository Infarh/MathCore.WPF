using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Converters.Base;

/// <summary>Математический конвертер одной переменной</summary>
[ValueConversion(typeof(double), typeof(double))]
public abstract class DoubleValueConverter : ValueConverter
{
    public virtual double? Min { get; set; }

    public virtual double? Max { get; set; }

    /// <summary>Преобразование объекта в вещественный тип данных</summary>
    /// <param name="obj">Преобразуемое значение</param>
    /// <returns>Значение вещественного типа, если преобразование прошло успешно, и NaN в противном случае</returns>
    public static double ConvertToDouble(object? obj)
    {
        if (obj is null || Equals(obj, Binding.DoNothing) || Equals(obj, DependencyProperty.UnsetValue)) return double.NaN;
        try
        {
            return System.Convert.ToDouble(obj);
        }
        catch (FormatException e)
        {
            Debug.WriteLine(e);
        }
        return double.NaN;
    }

    private static readonly NumberFormatInfo __NotInvarianceFormat = NumberFormatInfo.InvariantInfo.CloneObject(f => f.NumberDecimalSeparator = ",");

    /// <summary>Попытка преобразования объекта в вещественный тип данных</summary>
    /// <param name="obj">Преобразуемый объект</param>
    /// <param name="value">Выходное значение вещественного типа данных в случае успешного преобразования и NaN в противном случае</param>
    /// <returns>Результат успешности преобразования</returns>
    public static bool TryConvertToDouble(object? obj, out double value)
    {
        if (Equals(obj, Binding.DoNothing) || Equals(obj, DependencyProperty.UnsetValue))
        {
            value = double.NaN;
            return false;
        }

        switch (obj)
        {
            case string str when double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out value) || 
                                 double.TryParse(str, NumberStyles.Any, __NotInvarianceFormat, out value):
                return true;

            case sbyte x: value = x; return true;
            case byte x: value = x; return true;
            case short x: value = x; return true;
            case ushort x: value = x; return true;
            case int x: value = x; return true;
            case uint x: value = x; return true;
            case long x: value = x; return true;
            case ulong x: value = x; return true;
            case float x: value = x; return true;
            case double x: value = x; return true;

            case null:
                value = double.NaN;
                return false;
        }

        try
        {
            value = System.Convert.ToDouble(obj);
            return true;
        }
        catch (FormatException e)
        {
            Debug.WriteLine(e);
            value = double.NaN;
            return false;
        }
    }

    /// <summary>Преобразование значения</summary>
    /// <param name="v">Преобразуемое значение</param>
    /// <param name="p">Возможный параметр преобразования</param>
    /// <returns>Преобразованное значение</returns>
    protected abstract double Convert(double v, double? p = null);

    /// <summary>Обратное преобразование значения</summary>
    /// <param name="v">Преобразованное значение</param>
    /// <param name="p">Возможный параметр преобразования</param>
    /// <returns>Исходное значение</returns>
    /// <exception cref="NotSupportedException">Генерируется при отсутствии переопределения в классах наследниках</exception>
    protected virtual double ConvertBack(double v, double? p = null) => throw new NotSupportedException("Обратное преобразование не поддерживается");

    /// <summary>Проверка входных параметров</summary>
    /// <param name="v">Входное значение</param>
    /// <param name="p">Входной параметр</param>
    /// <param name="value">Входное значение, приведённое к вещественному типу данных</param>
    /// <param name="parameter">Входной параметр, приведённый к вещественному типу данных</param>
    private static bool CheckParameters(object? v, object? p, out double value, out double? parameter)
    {
        if (!TryConvertToDouble(v, out value))
        {
            parameter = null;
            return false;
        }
        parameter = p != null && TryConvertToDouble(p, out var pr) ? pr : null;
        return true;
    }

    /// <inheritdoc />
    protected override object Convert(object? v, Type t, object? p, CultureInfo c)
    {
        if (!CheckParameters(v, p, out var value, out var parameter))
            return double.NaN;

        var result = Convert(value, parameter);

        if (Min is { } min && !double.IsNaN(min))
            result = Math.Max(result, min);
        if (Max is { } max && !double.IsNaN(max))
            result = Math.Min(result, max);

        return result;
    }

    /// <inheritdoc />
    protected override object ConvertBack(object? v, Type t, object? p, CultureInfo c) => CheckParameters(v, p, out var value, out var parameter) ? ConvertBack(value, parameter) : double.NaN;
}