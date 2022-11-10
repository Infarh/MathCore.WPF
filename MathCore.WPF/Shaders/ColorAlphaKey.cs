using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Shaders;

/// <summary>Установка прозрачного цвета</summary>
public class ColorAlphaKey : ShaderEffect
{
    public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(nameof(Input), typeof(ColorAlphaKey), 0);

    [System.ComponentModel.Browsable(false)]
    public Brush Input { get => (Brush)GetValue(InputProperty); set => SetValue(InputProperty, value); }

    private static readonly PixelShader __PixelShader = new();

    static ColorAlphaKey()
    {
        using var data_stream = typeof(ColorAlphaKey).Assembly.GetManifestResourceStream($"{typeof(ColorAlphaKey).FullName}.fx.ps");
        __PixelShader.SetStreamSource(data_stream);
    }

    public ColorAlphaKey()
    {
        PixelShader = __PixelShader;
        UpdateShaderValue(InputProperty);
    }
}