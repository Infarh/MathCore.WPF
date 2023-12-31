using System.Windows.Markup;
using System.Windows.Media;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF;

/// <summary>Кеширование изображения</summary>
/// <remarks>Кеширование изображения</remarks>
/// <param name="scale"></param>
[MarkupExtensionReturnType(typeof(BitmapCache))]
public class VisualCache(double scale) : MarkupExtension
{
    /// <summary>Кеширование изображения</summary>
    public VisualCache() : this(1) { }

    /// <summary>Масштаб рендринга изображения</summary>
    public double Scale { get; set; } = scale;

    public bool ClearType { get; set; }

    /// <summary>Привязка к пикселям</summary>
    public bool RealPixels { get; set; }

    #region Overrides of MarkupExtension

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider sp) =>
        new BitmapCache
        {
            EnableClearType     = ClearType,
            RenderAtScale       = Scale,
            SnapsToDevicePixels = RealPixels
        };

    #endregion
}