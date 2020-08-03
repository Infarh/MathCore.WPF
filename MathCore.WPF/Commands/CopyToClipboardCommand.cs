using System.Windows;
using System.Windows.Media.Imaging;

namespace MathCore.WPF.Commands
{
    public class CopyToClipboardCommand : Command
    {
        public override bool CanExecute(object? parameter) => parameter is { };

        public override void Execute(object? parameter)
        {
            switch (parameter)
            {
                case null: break;
                case string str: Clipboard.SetText(str); break;
                case BitmapSource img: Clipboard.SetImage(img); break;
                default:
#pragma warning disable CA1062 // Проверить аргументы или открытые методы
                    var s = parameter.ToString();
#pragma warning restore CA1062 // Проверить аргументы или открытые методы
                    if (string.IsNullOrEmpty(s)) break;
                    Clipboard.SetText(s); 
                    break;
            }
        }
    }
}