using System.Drawing;
using System.Windows;
using System.Windows.Media.Effects;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Shaders
{
    /// <summary>Инвертирование цветов</summary>
    public class Invert : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(nameof(Input), typeof(Invert), 0);

        private static readonly PixelShader __PixelShader = new();

        static Invert()
        {
            using var data_stream = typeof(Invert).Assembly.GetManifestResourceStream(typeof(Invert).FullName!);
            __PixelShader.SetStreamSource(data_stream);
        }

        [System.ComponentModel.Browsable(false)]
        public Brush Input { get => (Brush)GetValue(InputProperty); set => SetValue(InputProperty, value); }

        public Invert()
        {
            PixelShader = __PixelShader;
            UpdateShaderValue(InputProperty);
        }
    }
}