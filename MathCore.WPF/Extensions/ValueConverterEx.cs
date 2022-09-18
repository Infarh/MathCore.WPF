using System.Windows.Data;

namespace MathCore.WPF.Extensions;

public static class ValueConverterExtensions
{
    public static IEnumerable<(Type SourceType, Type TargetType)> GetTypeConversion(this IValueConverter converter)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(converter);
#else
        if (converter is null) throw new ArgumentNullException(nameof(converter));
#endif

        var any = false;
        foreach (var attribute in converter.NotNull().GetType().GetCustomAttributes<ValueConversionAttribute>())
        {
            if (!any) any = true;
            yield return (attribute.SourceType, attribute.TargetType);
        }

        if (!any)
            yield return (typeof(object), typeof(object));
    }
}
