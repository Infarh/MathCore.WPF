using System;
using System.Windows.Markup;
using System.Windows.Media;

namespace MathCore.WPF
{
    /// <summary>Трансформация смещения</summary>
    [MarkupExtensionReturnType(typeof(TranslateTransform))]
    public class Translate : MarkupExtension
    {
        /// <summary>Смещение по X</summary>
        public double X { get; set; } = 1;
        /// <summary>Смещение по Y</summary>
        public double Y { get; set; } = 1;

        /// <summary>Трансформация смещения</summary>
        public Translate() { }

        /// <summary>Трансформация смещения</summary>
        /// <param name="x">Смещение по X</param>
        /// <param name="y">Смещение по Y</param>
        public Translate(double x, double y) { X = x; Y = y; }

        #region Overrides of MarkupExtension

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider) => new TranslateTransform { X = X, Y = Y };

        #endregion 
    }
}