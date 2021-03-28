using System;
using System.IO;
using System.Linq;
using System.Windows;

using Microsoft.Win32;

namespace MathCore.WPF.Services
{
    public class UserDialog : IUserDialogBase
    {
        protected static Window? ActiveWindow => Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsActive);
        protected static Window? FocusedWindow => Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsFocused);

        protected static Window? CurrentWindow => FocusedWindow ?? ActiveWindow;

        public FileInfo? OpenFile(string Title, string Filter = "Все файлы (*.*)|*.*", string? DefaultFilePath = null)
        {
            var dialog = new OpenFileDialog
            {
                Title = Title,
                RestoreDirectory = true,
                Filter = Filter ?? throw new ArgumentNullException(nameof(Filter)),
            };
            if (DefaultFilePath is { Length: > 0 })
                dialog.FileName = DefaultFilePath;

            return dialog.ShowDialog(CurrentWindow) == true
                ? new(dialog.FileName)
                : DefaultFilePath is null ? null : new(DefaultFilePath);
        }

        public FileInfo? SaveFile(string Title, string Filter = "Все файлы (*.*)|*.*", string? DefaultFilePath = null)
        {
            var dialog = new SaveFileDialog
            {
                Title = Title,
                RestoreDirectory = true,
                Filter = Filter ?? throw new ArgumentNullException(nameof(Filter)),
            };
            if (DefaultFilePath is { Length: > 0 })
                dialog.FileName = DefaultFilePath;

            return dialog.ShowDialog(CurrentWindow) == true
                ? new(dialog.FileName)
                : DefaultFilePath is null ? null : new(DefaultFilePath);
        }

        public bool YesNoQuestion(string Text, string Title = "Вопрос...")
        {
            var result = CurrentWindow is null
                ? MessageBox.Show(Text, Title, MessageBoxButton.YesNo, MessageBoxImage.Question)
                : MessageBox.Show(CurrentWindow, Text, Title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public bool OkCancelQuestion(string Text, string Title = "Вопрос...")
        {
            var result = CurrentWindow is null
                ? MessageBox.Show(Text, Title, MessageBoxButton.OKCancel, MessageBoxImage.Question)
                : MessageBox.Show(CurrentWindow, Text, Title, MessageBoxButton.OKCancel, MessageBoxImage.Question);
            return result == MessageBoxResult.OK;
        }

        public void Information(string Text, string Title = "Вопрос...")
        {
            if (CurrentWindow is null)
                MessageBox.Show(Text, Title, MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show(CurrentWindow, Text, Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Warning(string Text, string Title = "Вопрос...")
        {
            if (CurrentWindow is null)
                MessageBox.Show(Text, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                MessageBox.Show(CurrentWindow, Text, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void Error(string Text, string Title = "Вопрос...")
        {
            if (CurrentWindow is null)
                MessageBox.Show(Text, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show(CurrentWindow, Text, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}