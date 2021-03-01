using System.Drawing;
using System.Windows;
using System.Windows.Media.Effects;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Shaders
{
    /// <summary>Оттенки серого</summary>
    public class GrayScale : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(nameof(Input), typeof(GrayScale), 0);

        public static readonly DependencyProperty FactorProperty = 
            DependencyProperty.Register(
                nameof(Factor),
                typeof(float),
                typeof(GrayScale),
                new UIPropertyMetadata(1f, PixelShaderConstantCallback(0)));

        public float Factor { get => (float)GetValue(FactorProperty); set => SetValue(FactorProperty, value); }

        private static readonly PixelShader __PixelShader = new();

        static GrayScale()
        {
            using var data_stream = typeof(GrayScale).Assembly.GetManifestResourceStream(typeof(GrayScale).FullName!);
            __PixelShader.SetStreamSource(data_stream);
        }

        [System.ComponentModel.Browsable(false)]
        public Brush Input { get => (Brush)GetValue(InputProperty); set => SetValue(InputProperty, value); }

        public GrayScale()
        {
            PixelShader = __PixelShader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(FactorProperty);
        }
    }
}