using System.Windows;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.Extensions;

public static class DialogViewModelEx
{
    public static T ConnectTo<T>(this T model, Window DialogWindow) where T : DialogViewModel
    {
        model.Completed += (_, e) =>
        {
            DialogWindow.DialogResult = e;
            DialogWindow.Close();
        };

        return model;
    }
}

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
