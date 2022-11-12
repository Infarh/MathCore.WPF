using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Shaders;

/// <summary>Направленное размытие</summary>
public class DirectionalBlur : ShaderEffect
{
    public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(nameof(Input), typeof(DirectionalBlur), 0);

    [System.ComponentModel.Browsable(false)]
    public Brush Input { get => (Brush)GetValue(InputProperty); set => SetValue(InputProperty, value); }

    public static readonly DependencyProperty AngleProperty = 
        DependencyProperty.Register(
            nameof(Angle),
            typeof(double),
            typeof(DirectionalBlur),
            new UIPropertyMetadata(default(double), PixelShaderConstantCallback(0)));

    /// <summary>Направление в градусах</summary>
    public double Angle { get => (double)GetValue(AngleProperty); set => SetValue(AngleProperty, value); }

    public static readonly DependencyProperty BlurAmountProperty = 
        DependencyProperty.Register(
            nameof(BlurAmount),
            typeof(double),
            typeof(DirectionalBlur),
            new UIPropertyMetadata(0.1d, PixelShaderConstantCallback(1)));

    /// <summary>Размер размытия</summary>
    public double BlurAmount { get => (double)GetValue(BlurAmountProperty); set => SetValue(BlurAmountProperty, value); }

    private static readonly PixelShader __PixelShader = new();

    static DirectionalBlur()
    {
        using var data_stream = typeof(DirectionalBlur).Assembly.GetManifestResourceStream($"{typeof(DirectionalBlur).FullName}.fx.ps");
        __PixelShader.SetStreamSource(data_stream);
    }

    public DirectionalBlur()
    {
        PixelShader = __PixelShader;
        UpdateShaderValue(InputProperty);
    }
}