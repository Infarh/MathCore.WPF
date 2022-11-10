using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Shaders;

/// <summary>Размытие</summary>
public class Blur : ShaderEffect
{
    public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(nameof(Input), typeof(Blur), 0);

    [System.ComponentModel.Browsable(false)]
    public Brush Input { get => (Brush)GetValue(InputProperty); set => SetValue(InputProperty, value); }

    public static readonly DependencyProperty FactorProperty = 
        DependencyProperty.Register(
            nameof(Factor),
            typeof(float),
            typeof(Blur),
            new UIPropertyMetadata(0.5f, PixelShaderConstantCallback(0)));

    public float Factor { get => (float)GetValue(FactorProperty); set => SetValue(FactorProperty, value); }

    private static readonly PixelShader __PixelShader = new();

    static Blur()
    {
        using var data_stream = typeof(Blur).Assembly.GetManifestResourceStream($"{typeof(Blur).FullName}.fx.ps");
        __PixelShader.SetStreamSource(data_stream);
    }

    public Blur()
    {
        PixelShader = __PixelShader;
        UpdateShaderValue(InputProperty);
        UpdateShaderValue(FactorProperty);
    }
}