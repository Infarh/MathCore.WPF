using MathCore.WPF.Services;
using MathCore.WPF.WindowTest.Services.Interfaces;
using MathCore.WPF.WindowTest.ViewModels;
using MathCore.WPF.WindowTest.Views;

namespace MathCore.WPF.WindowTest.Services;

public class TestUserDialogService : UserDialogService, IUserDialog
{
    public void ShowTestDialog()
    {
        var model = new TestDialogViewModel();

        var dialog = new TestDialogWindow { DataContext = model };

        model.Completed += (_, e) =>
        {
            dialog.DialogResult = e;
            dialog.Close();
        };

        dialog.Show();
    }
}