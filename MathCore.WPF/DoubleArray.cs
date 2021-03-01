using System;
using System.Globalization;
using System.Linq;
using System.Windows.Markup;

namespace MathCore.WPF
{
    [MarkupExtensionReturnType(typeof(double[]))]
    public class DoubleArray : MarkupExtension
    {
        public string Data { get; set; } = "";
        public DoubleArray() { }
        public DoubleArray(string Data) => this.Data = Data;

        public override object ProvideValue(IServiceProvider ServiceProvider) =>
            Data.Split(new[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(s => s.Replace(',', '.'))
               .Select(s => double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : double.NaN)
               .WhereNot(double.IsNaN)
               .ToArray();
    }
}
