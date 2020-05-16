using System;
using System.Windows.Markup;
using System.Windows.Media;
using MathCore.Annotations;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedType.Global

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
        [NotNull]
        public override object ProvideValue(IServiceProvider sp) => new TranslateTransform { X = X, Y = Y };

        #endregion 
    }
}