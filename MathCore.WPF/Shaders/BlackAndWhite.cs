using System.Drawing;
using System.Windows;
using System.Windows.Media.Effects;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Shaders
{
    /// <summary>Чёрно-белое изображение</summary>
    public class BlackAndWhite : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(nameof(Input), typeof(BlackAndWhite), 0);

        [System.ComponentModel.Browsable(false)]
        public Brush Input { get => (Brush)GetValue(InputProperty); set => SetValue(InputProperty, value); }

        public static readonly DependencyProperty FactorProperty = 
            DependencyProperty.Register(
                nameof(Factor), 
                typeof(float), 
                typeof(BlackAndWhite), 
                new UIPropertyMetadata(1f, PixelShaderConstantCallback(0)));

        public float Factor { get => (float)GetValue(FactorProperty); set => SetValue(FactorProperty, value); }

        private static readonly PixelShader __PixelShader = new PixelShader();

        static BlackAndWhite()
        {
            using var data_stream = typeof(BlackAndWhite).Assembly.GetManifestResourceStream(typeof(BlackAndWhite).FullName!);
            __PixelShader.SetStreamSource(data_stream);
        }

        public BlackAndWhite()
        {
            PixelShader = __PixelShader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(FactorProperty);
        }
    }
}