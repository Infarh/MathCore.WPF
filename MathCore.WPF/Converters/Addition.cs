using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    /// <summary>��������������� �������� �������� � ������������ ������</summary>
    [MarkupExtensionReturnType(typeof(Addition))]
    public class Addition : SimpleDoubleValueConverter
    {
        public Addition() : this(0) { }
        public Addition(double P) : base(P, (v, p) => v + p, (r, p) => r - p) { }
    }
}