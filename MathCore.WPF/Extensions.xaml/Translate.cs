using System;
using System.Windows.Markup;
using System.Windows.Media;

namespace MathCore.WPF
{
    /// <summary>������������� ��������</summary>
    [MarkupExtensionReturnType(typeof(TranslateTransform))]
    public class Translate : MarkupExtension
    {
        /// <summary>�������� �� X</summary>
        public double X { get; set; } = 1;
        /// <summary>�������� �� Y</summary>
        public double Y { get; set; } = 1;

        /// <summary>������������� ��������</summary>
        public Translate() { }

        /// <summary>������������� ��������</summary>
        /// <param name="x">�������� �� X</param>
        /// <param name="y">�������� �� Y</param>
        public Translate(double x, double y) { X = x; Y = y; }

        #region Overrides of MarkupExtension

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider) => new TranslateTransform { X = X, Y = Y };

        #endregion 
    }
}