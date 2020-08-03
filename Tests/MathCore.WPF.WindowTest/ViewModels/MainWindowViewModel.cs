using System;
using System.Collections.Generic;
using System.Text;
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

        /// <summary>Тестовая команда</summary>
        public ICommand TestCommand { get; }

        /// <summary>Проверка возможности выполнения - Тестовая команда</summary>
        private static bool CanTestCommandExecute() => true;

        /// <summary>Логика выполнения - Тестовая команда</summary>
        private void OnTestCommandExecuted() => Status = $"Test {DateTime.Now:hh:mm:ss.ffff}";

        #endregion

        public IModelEvent StatusChangedEvent { get; }

        public MainWindowViewModel()
        {
            TestCommand = new LambdaCommand(OnTestCommandExecuted, CanTestCommandExecute);

            StatusChangedEvent = new ModelEvent(this);
        }
    }
}
