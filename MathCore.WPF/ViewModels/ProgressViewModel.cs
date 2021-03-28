using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;

using MathCore.WPF.Commands;
using MathCore.WPF.Services;

namespace MathCore.WPF.ViewModels
{
    public class ProgressViewModel : ViewModel, IProgressInfo
    {
        public ProgressViewModel() { }

        public ProgressViewModel(string Title) => _Title = Title;

        public event EventHandler? Disposed;

        #region Title : string - Заголовок окна

        /// <summary>Заголовок окна</summary>
        private string _Title = "Прогресс";

        /// <summary>Заголовок окна</summary>
        public string Title { get => _Title; set => Set(ref _Title, value); }

        #endregion

        #region StatusValue : string - Статус

        /// <summary>Статус</summary>
        private string _StatusValue = "Статус";

        /// <summary>Статус</summary>
        public string StatusValue { get => _StatusValue; set => Set(ref _StatusValue, value); }

        public void SetStatus(string value) => Set(ref _StatusValue, value, nameof(StatusValue));

        #endregion

        #region InformationValue : string - Информация

        /// <summary>Информация</summary>
        private string _InformationValue = "Информация";

        /// <summary>Информация</summary>
        public string InformationValue { get => _InformationValue; set => Set(ref _InformationValue, value); }

        public void SetInformation(string value) => Set(ref _InformationValue, value, nameof(InformationValue));

        #endregion

        #region ProgressValue : double - Прогресс

        /// <summary>Прогресс</summary>
        private double _ProgressValue = 0.5;

        /// <summary>Прогресс</summary>
        public double ProgressValue { get => _ProgressValue; set => Set(ref _ProgressValue, value); }

        public void SetProgress(double value) => Set(ref _ProgressValue, value, nameof(ProgressValue));

        #endregion

        #region ProgressMaxValue : double - Прогресс

        /// <summary>Прогресс</summary>
        private double _ProgressMaxValue = 1;

        /// <summary>Прогресс</summary>
        public double ProgressMaxValue { get => _ProgressMaxValue; set => Set(ref _ProgressMaxValue, value); }

        #endregion

        #region Command CancelCommand - Отмена операции

        /// <summary>Отмена операции</summary>
        private LambdaCommand? _CancelCommand;

        /// <summary>Отмена операции</summary>
        public ICommand CancelCommand => _CancelCommand ??= new(OnCancelCommandExecuted, CanCancelCommandExecute);

        /// <summary>Проверка возможности выполнения - Отмена операции</summary>
        private bool CanCancelCommandExecute() => _Cancellation != null;

        /// <summary>Логика выполнения - Отмена операции</summary>
        private void OnCancelCommandExecuted() => _Cancellation?.Cancel();

        #endregion

        private Progress<double>? _Progress;

        public IProgress<double> Progress => _Progress ??= Application.Current.Dispatcher.Invoke(() => new Progress<double>(SetProgress));

        private Progress<string>? _Information;

        public IProgress<string> Information => _Information ??= Application.Current.Dispatcher.Invoke(() => new Progress<string>(SetInformation));

        private Progress<string>? _Status;

        public IProgress<string> Status => _Status ??= Application.Current.Dispatcher.Invoke(() => new Progress<string>(SetStatus));

        private CancellationTokenSource? _Cancellation;

        public CancellationToken Cancel => (_Cancellation ??= new()).Token;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;
            if (_Cancellation is { } cancellation)
            {
                cancellation.Cancel();
                cancellation.Dispose();
            }
            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }
}
