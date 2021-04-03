using System;
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

        /// <summary>Тестовая команда</summary>
        private Command? _TestCommand;

        /// <summary>Тестовая команда</summary>
        public ICommand TestCommand => _TestCommand ??= Command.New(OnTestCommandExecuted);

        /// <summary>Логика выполнения - Тестовая команда</summary>
        private static void OnTestCommandExecuted()
        {
            var service = App.Services.GetRequiredService<IUserDialogBase>();

            var input_text = service.GetText("Сообщение", "Диалог", "Текст по умолчанию");

            if (input_text != null)
                service.Information(input_text, "Введённый текст");
            else
                service.Warning("Текст не введён", "Предупреждение!");

        }
    }
}

