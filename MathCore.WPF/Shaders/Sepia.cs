using System.Drawing;
using System.Windows;
using System.Windows.Media.Effects;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Shaders
{
    /// <summary>Сепия</summary>
    public class Sepia : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(nameof(Input), typeof(Sepia), 0);

        [System.ComponentModel.Browsable(false)]
        public Brush Input { get => (Brush)GetValue(InputProperty); set => SetValue(InputProperty, value); }

        public static readonly DependencyProperty FactorProperty =
            DependencyProperty.Register(
                nameof(Factor),
                typeof(float),
                typeof(Sepia),
                new UIPropertyMetadata(0.5f, PixelShaderConstantCallback(0)));

        public float Factor { get => (float)GetValue(FactorProperty); set => SetValue(FactorProperty, value); }

        private static readonly PixelShader __PixelShader = new PixelShader();

        static Sepia()
        {
            using var data_stream = typeof(Sepia).Assembly.GetManifestResourceStream(typeof(Sepia).FullName!);
            __PixelShader.SetStreamSource(data_stream);
        }

        public Sepia()
        {
            PixelShader = __PixelShader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(FactorProperty);
        }
    }
}