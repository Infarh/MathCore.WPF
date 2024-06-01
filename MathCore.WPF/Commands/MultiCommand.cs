using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Markup;
using MathCore.WPF.ViewModels;
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable UnusedType.Global

// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Commands;

[ContentProperty("Commands")]
public class MultiCommand(params ICommand[] commands) : LambdaCommand, IAddChild
{
    public MultiCommand() : this([]) { }

    private readonly Collection<ICommand> _Commands = new(commands);

    private bool _ExecuteIndependently = true;

    public bool ExecuteIndependently
    {
        get => _ExecuteIndependently;
        set => Set(ref _ExecuteIndependently, value);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Collection<ICommand> Commands => _Commands;


    /// <inheritdoc />
    public override bool CanExecute(object? parameter)
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
    public override void Execute(object? parameter)
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
        if(value is null) throw new ArgumentNullException(nameof(value));
        if(value is not ICommand command)
            throw new ArgumentException($"Не допускается добавления значений тип {value.GetType()}", nameof(value));
        _Commands.Add(command);
    }

    /// <inheritdoc />
    public void AddText(string text) => throw new NotSupportedException("Множественная команда не допускает добавления текстовых данных");
}