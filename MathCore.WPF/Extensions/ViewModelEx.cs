using System.Windows;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.Extensions;

internal static class ViewModelEx
{
    public static T SetViewModel<T>(this T Window, ViewModel Model) where T : Window
    {
        Window.DataContext = Model;
        return Window;
    }

    public static T SetViewModel<T>(this T Window, DialogViewModel Model) where T : Window =>
        Window.SetViewModel((ViewModel)Model.ConnectTo(Window));
}
