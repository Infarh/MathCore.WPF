using System.Globalization;
using System.Text;
using System.Windows.Data;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

public class ArrayToStringConverter : ValueConverter
{
    protected override object? Convert(object? v, Type t, object? p, CultureInfo c)
    {
        if (v is not Array array)
            return Binding.DoNothing;

        var result = new StringBuilder();
        for (var i = 0; i < array.Length; i++)
        {
            result.Append(array.GetValue(i));
            result.Append(',');
        }

        if (result.Length > 0)
            result.Length--;

        return result.ToString();
    }
}
