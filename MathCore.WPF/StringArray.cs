using System;
using System.Windows.Markup;

namespace MathCore.WPF
{
    [MarkupExtensionReturnType(typeof(string[]))]
    public class StringArray : MarkupExtension
    {
        private string _Data;

        public string Data { get => _Data; set => _Data = value; }

        public char Separator { get; set; }
        public bool RemoveEmpty { get; set; }

        public StringArray() { }
        public StringArray(string Data) => _Data = Data;

        public override object ProvideValue(IServiceProvider serviceProvider) => _Data.Split(new[] { Separator }, RemoveEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
    }
}