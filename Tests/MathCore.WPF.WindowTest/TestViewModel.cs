using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

using MathCore.WPF.Commands;
using MathCore.WPF.Services;
using MathCore.WPF.ViewModels;
using MathCore.WPF.WindowTest.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace MathCore.WPF.WindowTest
{
    [MarkupExtensionReturnType(typeof(TestViewModel))]
    class TestViewModel : ViewModel
    {
        public Graph Graph { get; } = new Graph();

        public TestViewModel()
        {
            var rnd = new Random();
            var graph = Graph;
            for (var i = 0; i < 20; i++)
                graph.Add(new Node { Position = new Point(rnd.Next(10, 491), rnd.Next(10, 491)), Radius = rnd.Next(2, 11) });
        }

        #region Command TestCommand - Тестовая команда

        /// <summary>Тестовая команда</summary>
        private Command? _TestCommand;

        /// <summary>Тестовая команда</summary>
        public ICommand TestCommand => _TestCommand ??= Command.New(OnTestCommandExecutedAsync, CanTestCommandExecute);

        /// <summary>Проверка возможности выполнения - Тестовая команда</summary>
        private static bool CanTestCommandExecute() => true;

        /// <summary>Логика выполнения - Тестовая команда</summary>
        private static async Task OnTestCommandExecutedAsync()
        {
            var service = App.Services.GetRequiredService<IUserDialogBase>();
            using var progress_dialog = service.Progress("Прогресс", "Прогресс операции");

            var cancel = CancellationToken.None;
            IProgress<double>? progress = null;
            for (var i = 0; i < 100; i++)
            {
                if (i == 50) cancel = progress_dialog.Cancel;
                if(i == 30) progress = progress_dialog.Progress;

                progress?.Report((i + 1) / 100d);
                await Task.Delay(100, cancel);
                progress_dialog.Information.Report($"Итерация {i}");
            }
        }

        #endregion
    }
}
