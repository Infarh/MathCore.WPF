using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MathCore.WPF.Commands
{
    public class RenderImageToClipboardCommand : LambdaCommand<FrameworkElement>
    {
        public static double PictureFactor { get; set; } = 1;

        /// <inheritdoc />
        public RenderImageToClipboardCommand()
        {
            _ExecuteAction = RenderElement;
            _CanExecute = e => e != null;
        }

        private void RenderElement(FrameworkElement e)
        {
            var height = e.ActualHeight;
            var width = e.ActualWidth;
            e.Arrange(new Rect(0, 0, width, height));
            //(e as ParameterViewer)?.DrawAll();
            e.UpdateLayout();

            var bitmap_height = height * PictureFactor;
            var bitmap_widht = width * PictureFactor;

            var bitmap = new RenderTargetBitmap((int)bitmap_widht, (int)bitmap_height, 96 * bitmap_widht / width, 96 * bitmap_height / height, PixelFormats.Default);
            bitmap.Render(e);
            Clipboard.SetImage(bitmap);
        }
    }
}