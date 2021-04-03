﻿using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

using MathCore.WPF.Commands;
using MathCore.WPF.Services;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace MathCore.WPF.ViewModels
{
    /// <summary>Модель диалога прогресса</summary>
    [MarkupExtensionReturnType(typeof(ProgressViewModel))]
    public class ProgressViewModel : ViewModel, IProgressInfo
    {
        /// <summary>Событие возникает в момент вызова отмены операции в диалоге (вызове команды отмены)</summary>
        public event EventHandler? Cancelled;

        /// <summary>Когда операция отменяется</summary>
        /// <param name="e">Аргумент события</param>
        protected virtual void OnCancelled(EventArgs? e)
        {
            if (!_IsDisposed)
                Cancelled?.Invoke(this, e ?? EventArgs.Empty);
        }

        public ProgressViewModel() { }

        public ProgressViewModel(string Title) => _Title = Title;

        public event EventHandler? Disposed;

        #region Title : string - Заголовок окна

        /// <summary>Заголовок окна</summary>
        private string _Title;

        /// <summary>Заголовок окна</summary>
        public string Title { get => _Title; set => Set(ref _Title, value); }

        #endregion

        #region StatusValue : string? - Статус

        /// <summary>Статус</summary>
        private string? _StatusValue;

        /// <summary>Статус</summary>
        public string? StatusValue { get => _StatusValue; set => Set(ref _StatusValue, value); }

        public void SetStatus(string? value) => Set(ref _StatusValue, value, nameof(StatusValue));

        #endregion

        #region InformationValue : string? - Информация

        /// <summary>Информация</summary>
        private string? _InformationValue;

        /// <summary>Информация</summary>
        public string? InformationValue { get => _InformationValue; set => Set(ref _InformationValue, value); }

        public void SetInformation(string? value) => Set(ref _InformationValue, value, nameof(InformationValue));

        #endregion

        #region ProgressValue : double - Прогресс

        /// <summary>Прогресс</summary>
        private double _ProgressValue = double.NaN;

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

        public bool Cancellable => _Cancellation != null;

        #region Command CancelCommand - Отмена операции

        /// <summary>Отмена операции</summary>
        private Command? _CancelCommand;

        /// <summary>Отмена операции</summary>
        public ICommand CancelCommand => _CancelCommand ??= Command.New(OnCancelCommandExecuted, CanCancelCommandExecute);

        /// <summary>Проверка возможности выполнения - Отмена операции</summary>
        private bool CanCancelCommandExecute() => _Cancellation != null;

        /// <summary>Логика выполнения - Отмена операции</summary>
        private void OnCancelCommandExecuted()
        {
            _Cancellation?.Cancel();
            OnCancelled(EventArgs.Empty);
        }

        #endregion

        private Progress<double>? _Progress;

        public IProgress<double> Progress => _Progress ??= Application.Current.Dispatcher.Invoke(() => new Progress<double>(SetProgress));

        private Progress<string>? _Information;

        public IProgress<string> Information => _Information ??= Application.Current.Dispatcher.Invoke(() => new Progress<string>(SetInformation));

        private Progress<string>? _Status;

        public IProgress<string> Status => _Status ??= Application.Current.Dispatcher.Invoke(() => new Progress<string>(SetStatus));

        private CancellationTokenSource? _Cancellation;

        public CancellationToken Cancel
        {
            get
            {
                if (_Cancellation is not null) return _Cancellation!.Token;
                Set(ref _Cancellation, new(), nameof(Cancellable));
                CommandManager.InvalidateRequerySuggested();
                return _Cancellation!.Token;
            }
        }

        #region IsDisposed : bool - Модель разрушена

        /// <summary>Модель разрушена</summary>
        private bool _IsDisposed;

        /// <summary>Модель разрушена</summary>
        public bool IsDisposed { get => _IsDisposed; private set => Set(ref _IsDisposed, value); }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing || _IsDisposed) return;
            IsDisposed = true;
            if (_Cancellation is { } cancellation)
            {
                cancellation.Cancel();
                cancellation.Dispose();
                _Cancellation = null;
            }
            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }
}
