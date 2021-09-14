using System;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using MathCore.Annotations;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF
{
    /// <summary>Эффект тени</summary>
    [MarkupExtensionReturnType(typeof(DropShadowEffect))]
    public class Shadow : MarkupExtension
    {
        /// <summary>Глубина тени</summary>
        public double Depth { get; set; } = 5;

        /// <summary>Алгоритм рендринга</summary>
        public RenderingBias Bias { get; set; } = RenderingBias.Performance;

        /// <summary>Прозрачность</summary>
        public double Opacity { get; set; } = 1;

        /// <summary>Направление</summary>
        public double Direction { get; set; } = 315;

        /// <summary>Цвет</summary>
        public Color Color { get; set; } = Colors.Black;

        /// <summary>Размытие</summary>
        public double Blur { get; set; } = 5;

        /// <summary>Эффект тени</summary>
        public Shadow() { }

        /// <summary>Эффект тени</summary>
        /// <param name="depth">Глубина</param>
        public Shadow(double depth) => Depth = depth;

        #region Overrides of MarkupExtension

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider sp) =>
            new DropShadowEffect
            {
                BlurRadius = Blur,
                Color = Color,
                Direction = Direction,
                Opacity = Opacity,
                RenderingBias = Bias,
                ShadowDepth = Depth
            };

        #endregion  
    }
}