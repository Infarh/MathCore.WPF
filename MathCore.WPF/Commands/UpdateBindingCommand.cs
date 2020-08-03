using System;
using System.Windows;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Commands
{
    public class UpdateBindingCommand : Command
    {
        public DependencyProperty? Property { get; set; }

        /// <inheritdoc />
        public override void Execute(object? parameter) => 
            (parameter as FrameworkElement)?
           .GetBindingExpression(Property ?? throw new InvalidOperationException("Свойство не определено"))?
           .UpdateSource();

        /// <inheritdoc />
        public override bool CanExecute(object? parameter) =>
            Property != null
            && parameter is FrameworkElement item
            && item.GetBindingExpression(Property) != null;
    }
}