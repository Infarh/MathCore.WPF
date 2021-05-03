using System;
// ReSharper disable UnusedType.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Commands
{
    public class EventCommand : LambdaCommand
    {
        public event Action<object?>? ExecuteEvent;
        public event Func<object, bool>? CanExecuteEvent;

        public bool CanExecuteIfEventNotDefined { get; set; } = true;

        /// <inheritdoc />
        public override void Execute(object? parameter) => ExecuteEvent?.Invoke(parameter);

        /// <inheritdoc />
        public override bool CanExecute(object? parameter) => base.CanExecute(parameter) && (CanExecuteEvent?.Invoke(parameter) ?? CanExecuteIfEventNotDefined);
    }
}