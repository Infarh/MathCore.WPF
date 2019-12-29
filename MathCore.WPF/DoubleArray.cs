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
        public DoubleArray(string Data) { this.Data = Data; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Data.Split(new[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Replace(',', '.'))
                .Select(s => { return double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? (double?)v : null; })
                .Where(v => v.HasValue)
                .Select(v => v.Value)
                .ToArray();
        }
    }
}
