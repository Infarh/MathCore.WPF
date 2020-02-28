using System;
using System.Windows.Markup;
using System.Windows.Media;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF
{
    /// <summary>Трансформация поворота</summary>
    [MarkupExtensionReturnType(typeof(RotateTransform))]
    public class Rotate : MarkupExtension
    {
        /// <summary>Угол</summary>
        public double Angle { get; set; }

        /// <summary>X-центра</summary>
        public double X { get; set; }

        /// <summary>Y-центра</summary>
        public double Y { get; set; }

        /// <summary>Трансформация поворота</summary>
        public Rotate() { }

        /// <summary>Трансформация поворота</summary>
        /// <param name="angle">Угол</param>
        public Rotate(double angle) { Angle = angle; }

        #region Overrides of MarkupExtension

        /// <inheritdoc />
        [NotNull]
        public override object ProvideValue(IServiceProvider sp) => new RotateTransform { Angle = Angle, CenterX = X, CenterY = Y };

        #endregion 
    }
}