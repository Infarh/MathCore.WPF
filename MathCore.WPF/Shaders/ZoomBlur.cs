using System.Drawing;
using System.Windows;
using System.Windows.Media.Effects;
using Point = System.Windows.Point;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Shaders
{
    /// <summary>Размытие из центра</summary>
    public class ZoomBlur : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(nameof(Input), typeof(ZoomBlur), 0);

        [System.ComponentModel.Browsable(false)]
        public Brush Input { get => (Brush)GetValue(InputProperty); set => SetValue(InputProperty, value); }

        public static readonly DependencyProperty CenterProperty = 
            DependencyProperty.Register(
                nameof(Center),
                typeof(Point),
                typeof(ZoomBlur),
                new UIPropertyMetadata(new Point(0.5D, 0.5D), PixelShaderConstantCallback(0)));

        public Point Center { get => (Point)GetValue(CenterProperty); set => SetValue(CenterProperty, value); }

        public static readonly DependencyProperty BlurAmountProperty = 
            DependencyProperty.Register(
                nameof(BlurAmount),
                typeof(double),
                typeof(ZoomBlur),
                new UIPropertyMetadata(0.1d, PixelShaderConstantCallback(1)));

        public double BlurAmount { get => (double)GetValue(BlurAmountProperty); set => SetValue(BlurAmountProperty, value); }

        private static readonly PixelShader __PixelShader = new PixelShader();

        static ZoomBlur()
        {
            using var data_stream = typeof(ZoomBlur).Assembly.GetManifestResourceStream(typeof(ZoomBlur).FullName!);
            __PixelShader.SetStreamSource(data_stream);
        }

        public ZoomBlur()
        {
            PixelShader = __PixelShader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(CenterProperty);
            UpdateShaderValue(BlurAmountProperty);
        }
    }
}