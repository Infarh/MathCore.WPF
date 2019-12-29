using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Markup;
using MathCore.WPF.ViewModels;

// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Commands
{
    [ContentProperty("Commands")]
    public class MultyCommand : LambdaCommand, IAddChild
    {
        private readonly Collection<ICommand> _Commands = new Collection<ICommand>();

        private bool _ExecuteIndependently = true;

        public bool ExecuteIndependently
        {
            get => _ExecuteIndependently;
            set => Set(ref _ExecuteIndependently, value);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<ICommand> Commands => _Commands;

        public MultyCommand() { }

        public MultyCommand(params ICommand[] commands) => _Commands.AddItems(commands);

        /// <inheritdoc />
        public override bool CanExecute(object parameter = null)
        {
            if (ViewModel.IsDesignMode) return true;
            if(_ExecuteIndependently)
            {
                foreach(var command in _Commands)
                    if(!command.CanExecute(parameter))
                        return false;
                return true;
            }
            foreach(var command in _Commands)
                if(command.CanExecute(parameter))
                    return true;
            return false;
        }

        /// <inheritdoc />
        public override void Execute(object parameter = null)
        {
            if(!_ExecuteIndependently)
                foreach(var command in _Commands)
                    command.Execute(parameter);
            else
                foreach(var command in _Commands)
                    if(command.CanExecute(parameter))
                        command.Execute(parameter);
        }

        /// <inheritdoc />
        public void AddChild(object value)
        {
            if(value == null) throw new ArgumentNullException(nameof(value));
            var command = value as ICommand;
            if(command == null)
                throw new ArgumentException($"Не допускается добавления значений тип {value.GetType()}", nameof(value));
            _Commands.Add(command);
        }

        /// <inheritdoc />
        public void AddText(string text) => throw new NotSupportedException("Множественная команда не допускает добавления текстовых данных");
    }
}