using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;

using MathCore.WPF.Commands;
using MathCore.WPF.UIEvents;
using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels
{
    [MarkupExtensionReturnType(typeof(MainWindowViewModel))]
    class MainWindowViewModel : ViewModel
    {
        #region Title : string - Заголовок

        /// <summary>Заголовок</summary>
        private string _Title = "Заголовок!";

        /// <summary>Заголовок</summary>
        public string Title { get => _Title; set => Set(ref _Title, value); }

        #endregion

        #region Status : string - Статус

        /// <summary>Статус</summary>
        private string _Status = "Ready!";

        /// <summary>Статус</summary>
        public string Status
        {
            get => _Status;
            set => SetValue(ref _Status, value).Then(StatusChangedEvent.Invoke);
        }

        #endregion

        #region Command TestCommand - Тестовая команда

        private ICommand? _TestCommand;

        /// <summary>Тестовая команда</summary>
        public ICommand TestCommand => _TestCommand ??= Command.New(OnTestCommandExecuted, CanTestCommandExecute);

        /// <summary>Проверка возможности выполнения - Тестовая команда</summary>
        private static bool CanTestCommandExecute(object? p) => true;

        /// <summary>Логика выполнения - Тестовая команда</summary>
        private async Task OnTestCommandExecuted(object? p) => Status = $"Test {DateTime.Now:hh:mm:ss.ffff}";

        #endregion

        #region Command ObjectCommandCommand - Summary

        /// <summary>Summary</summary>
        private LambdaCommand? _ObjectCommandCommand;

        /// <summary>Summary</summary>
        public ICommand ObjectCommandCommand => _ObjectCommandCommand ??= (OnObjectCommandCommandExecuted, CanObjectCommandCommandExecute);

        /// <summary>Проверка возможности выполнения - Summary</summary>
        private bool CanObjectCommandCommandExecute(object? p) => true;

        /// <summary>Логика выполнения - Summary</summary>
        private void OnObjectCommandCommandExecuted(object? p)
        {
            
        }

        #endregion

        #region Command StringCommand : string - Summary

        /// <summary>Summary</summary>
        private LambdaCommand<string>? _StringCommand;

        /// <summary>Summary</summary>
        public ICommand StringCommand => _StringCommand ??= (OnStringCommandExecuted, CanStringCommandExecute);

        /// <summary>Проверка возможности выполнения - Summary</summary>
        private bool CanStringCommandExecute(string p) => true;

        /// <summary>Проверка возможности выполнения - Summary</summary>
        private void OnStringCommandExecuted(string p)
        {
            
        }

        #endregion

        public IModelEvent StatusChangedEvent { get; }

        public MainWindowViewModel() => StatusChangedEvent = new ModelEvent(this);
    }
}
