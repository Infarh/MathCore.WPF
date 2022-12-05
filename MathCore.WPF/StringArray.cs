using System.Windows.Markup;

namespace MathCore.WPF;

[MarkupExtensionReturnType(typeof(string[]))]
public class StringArray : MarkupExtension
{
    private string _Data;

    public string Data { get => _Data; set => _Data = value; }

    public char Separator { get; set; }
    public bool RemoveEmpty { get; set; }

    public StringArray() { }
    public StringArray(string Data) => _Data = Data;

    public override object ProvideValue(IServiceProvider sp)
    {
        var result = new List<string>();
        foreach (var str in _Data.AsStringPtr().Split(RemoveEmpty, Separator))
            result.Add(str);
        return result.ToArray();
    }
}