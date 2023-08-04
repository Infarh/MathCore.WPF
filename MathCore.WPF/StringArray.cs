using System.Windows.Markup;

namespace MathCore.WPF;

[MarkupExtensionReturnType(typeof(string[]))]
public class StringArray(string Data) : MarkupExtension
{

    public string Data { get; set; } = Data;

    public char Separator { get; set; }

    public bool RemoveEmpty { get; set; }

    public StringArray() : this(string.Empty) { }

    public override object ProvideValue(IServiceProvider sp)
    {
        var result = new List<string>();
        foreach (var str in Data.AsStringPtr().Split(RemoveEmpty, Separator))
            result.Add(str);
        return result.ToArray();
    }
}