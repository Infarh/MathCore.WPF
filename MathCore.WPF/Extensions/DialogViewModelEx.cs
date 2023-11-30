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
