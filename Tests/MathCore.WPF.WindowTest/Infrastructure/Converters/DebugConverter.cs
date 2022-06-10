using System.Diagnostics;
using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.WindowTest.Infrastructure.Converters;

[MarkupExtensionReturnType(typeof(DebugConverter))]
public class DebugConverter : ValueConverter
{
    public bool Enabled { get; set; } = true;

    protected override object? Convert(object? v, Type t, object? p, CultureInfo c)
    {
        if(Enabled)
            Debugger.Break();
        return v;
    }
}
