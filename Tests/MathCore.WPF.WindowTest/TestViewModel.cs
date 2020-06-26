using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;
using MathCore.WPF.WindowTest.Models;

namespace MathCore.WPF.WindowTest
{
    [MarkupExtensionReturnType(typeof(TestViewModel))]
    class TestViewModel : ViewModel
    {
        private const int __CommandsCount = 100;

        public TitledCommand[] Commands { get; }

        public TestViewModel()
        {
            Action command_action = OnCommandExecuted;
            Commands = Enumerable
               .Range(1, __CommandsCount)
               .Select(i => new TitledCommand(command_action) {Title = $"C{i}"})
               .ToArray();
        }

        private static readonly Random __Rnd = new Random();

        private void OnCommandExecuted()
        {
            for (var i = 0; i < __CommandsCount / 10; i++)
            {
                var cmd = Commands[__Rnd.Next(0, __CommandsCount)];
                cmd.CanExecute ^= true;
            }
        }
    }
}
