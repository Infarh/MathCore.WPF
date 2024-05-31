using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Commands;

public class RenderImageToClipboardCommand : LambdaCommand<FrameworkElement>
{
    public static double PictureFactor { get; set; } = 1;

    /// <inheritdoc />
    public RenderImageToClipboardCommand()
    {
        _ExecuteAction = RenderElement;
        _CanExecute = e => e != null;
    }

    private static void RenderElement(FrameworkElement e)
    {
        var height = e.ActualHeight;
        var width = e.ActualWidth;
        e.Arrange(new(0, 0, width, height));
        e.UpdateLayout();

        var bitmap_height = height * PictureFactor;
        var bitmap_width = width * PictureFactor;

        var bitmap = new RenderTargetBitmap(
            pixelWidth: (int)bitmap_width,
            pixelHeight: (int)bitmap_height, 
            dpiX: 96 * bitmap_width / width,
            dpiY: 96 * bitmap_height / height,
            pixelFormat: PixelFormats.Default);
        bitmap.Render(e);
        Clipboard.SetImage(bitmap);
    }
}