using System.Globalization;
using System.Windows.Markup;

namespace MathCore.WPF;

[MarkupExtensionReturnType(typeof(double[]))]
public class DoubleArray : MarkupExtension
{
    public string Data { get; set; } = "";
    
    public DoubleArray() { }
    
    public DoubleArray(string Data) => this.Data = Data;

    public override object ProvideValue(IServiceProvider ServiceProvider)
    {
        var values = new List<double>();
        foreach (var item in Data.AsStringPtr().Split(';', ' '))
            if(item.Length > 0 && double.TryParse(item.ToString().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                values.Add(v);

        return values.ToArray();
    }
}