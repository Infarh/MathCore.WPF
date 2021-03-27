using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Shaders
{
    /// <summary>Прозрачность</summary>
    public class Opacity : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(nameof(Input), typeof(Opacity), 0);

        [System.ComponentModel.Browsable(false)]
        public Brush Input { get => (Brush)GetValue(InputProperty); set => SetValue(InputProperty, value); }

        public static readonly DependencyProperty FactorProperty = 
            DependencyProperty.Register(
                nameof(Factor),
                typeof(float),
                typeof(Opacity),
                new UIPropertyMetadata(0.5f, PixelShaderConstantCallback(0)));

        public float Factor { get => (float)GetValue(FactorProperty); set => SetValue(FactorProperty, value); }

        private static readonly PixelShader __PixelShader = new();

        static Opacity()
        {
            using var data_stream = typeof(Opacity).Assembly.GetManifestResourceStream($"{typeof(Opacity).FullName}.fx.ps");
            __PixelShader.SetStreamSource(data_stream);
        }

        public Opacity()
        {
            PixelShader = __PixelShader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(FactorProperty);
        }
    }
}