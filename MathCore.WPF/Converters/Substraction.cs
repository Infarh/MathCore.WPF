using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    /// <summary>��������������� ��������� ������������� ����� �� ��������</summary>
    [MarkupExtensionReturnType(typeof(Substraction))]
    public class Substraction : SimpleDoubleValueConverter
    {
        public Substraction() : this(0) { }
        public Substraction(double P) : base(P, (v, p) => v - p, (r, p) => r + p) { }
    }
}