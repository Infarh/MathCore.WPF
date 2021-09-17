using System.Windows;
using System.Windows.Input;

using MathCore.WPF.Commands;
using MathCore.WPF.Dialogs;
using MathCore.WPF.ViewModels;
using MathCore.WPF.WindowTest.Services.Interfaces;

namespace MathCore.WPF.WindowTest.ViewModels
{
    public class TestWindow3ViewModel : TitledViewModel
    {
        private readonly IUserDialog _UserDialog;

        public TestWindow3ViewModel(IUserDialog UserDialog)
        {
            _UserDialog = UserDialog;
            Title = "Тестовое окно 3";
        }

        #region Command ShowTestDialogCommand - Показать тестовый диалог

        /// <summary>Показать тестовый диалог</summary>
        private Command? _ShowTestDialogCommand;

        /// <summary>Показать тестовый диалог</summary>
        public ICommand ShowTestDialogCommand => _ShowTestDialogCommand ??= Command.New(() => _UserDialog.ShowTestDialog());

        #endregion

        #region Command CheckDialogCommand - Проверка работы диалога

        /// <summary>Проверка работы диалога</summary>
        private Command? _CheckDialogCommandCommand;

        /// <summary>Проверка работы диалога</summary>
        public ICommand CheckDialogCommand => _CheckDialogCommandCommand ??= Command.New(OnCheckDialogCommandExecuted);

        /// <summary>Логика выполнения - Проверка работы диалога</summary>
        private static void OnCheckDialogCommandExecuted()
        {
            var dialog = new SelectDirectory
            {

            };

            if(dialog.ShowDialog() != true) return;

            MessageBox.Show(dialog.SelectedDirectory.ToString());
        }

        #endregion
    }
}
