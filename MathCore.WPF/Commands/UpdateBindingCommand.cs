using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using MathCore.Annotations;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Commands
{
    public class UpdateBindingCommand : MarkupExtension, ICommand
    {
        /// <inheritdoc />
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value; 
            remove => CommandManager.RequerySuggested += value;
        }

        public DependencyProperty? Property { get; set; }

        /// <inheritdoc />
        public void Execute(object parameter) => (parameter as FrameworkElement)?.GetBindingExpression(Property)?.UpdateSource();

        /// <inheritdoc />
        public bool CanExecute(object? parameter) =>
            Property != null
            && parameter is FrameworkElement item
            && item.GetBindingExpression(Property) != null;

        /// <inheritdoc />
        [NotNull]
        public override object ProvideValue(IServiceProvider sp) => this;
    }
}