using System;
using System.Windows.Markup;
using System.Windows.Media;

namespace MathCore.WPF
{
    /// <summary>������������� ��������</summary>
    [MarkupExtensionReturnType(typeof(RotateTransform))]
    public class Rotate : MarkupExtension
    {
        /// <summary>����</summary>
        public double Angle { get; set; }

        /// <summary>X-������</summary>
        public double X { get; set; }

        /// <summary>Y-������</summary>
        public double Y { get; set; }

        /// <summary>������������� ��������</summary>
        public Rotate() { }

        /// <summary>������������� ��������</summary>
        /// <param name="angle">����</param>
        public Rotate(double angle) { Angle = angle; }

        #region Overrides of MarkupExtension

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider) =>
            new RotateTransform { Angle = Angle, CenterX = X, CenterY = Y };

        #endregion 
    }
}