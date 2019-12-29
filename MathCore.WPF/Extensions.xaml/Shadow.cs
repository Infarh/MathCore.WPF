using System;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace MathCore.WPF
{
    /// <summary>������ ����</summary>
    [MarkupExtensionReturnType(typeof(DropShadowEffect))]
    public class Shadow : MarkupExtension
    {
        /// <summary>������� ����</summary>
        public double Depth { get; set; } = 5;

        /// <summary>�������� ���������</summary>
        public RenderingBias Bias { get; set; } = RenderingBias.Performance;

        /// <summary>������������</summary>
        public double Opacity { get; set; } = 1;

        /// <summary>�����������</summary>
        public double Direction { get; set; } = 315;

        /// <summary>����</summary>
        public Color Color { get; set; } = Colors.Black;

        /// <summary>��������</summary>
        public double Blur { get; set; } = 5;

        /// <summary>������ ����</summary>
        public Shadow() { }

        /// <summary>������ ����</summary>
        /// <param name="depth">�������</param>
        public Shadow(double depth) { Depth = depth; }

        #region Overrides of MarkupExtension

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider) =>
            new DropShadowEffect { BlurRadius = Blur, Color = Color, Direction = Direction, Opacity = Opacity, RenderingBias = Bias, ShadowDepth = Depth };

        #endregion  
    }
}