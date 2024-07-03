using System.Windows;
using System.Windows.Markup;

namespace MathCore.WPF.Commands;

[MarkupExtensionReturnType(typeof(SetVisualStateCommand))]
public class SetVisualStateCommand(string State) : Command
{
    public SetVisualStateCommand() : this("") { }

    public string State { get; set; } = State;

    public bool UseTransition { get; set; }

    public bool CheckStateExists { get; set; }

    public override bool CanExecute(object? parameter)
    {
        if (!State.IsNotNullOrEmpty()) return false;
        if (parameter is not FrameworkElement element) return false;

        if (!CheckStateExists) return true;

        if (VisualStateManager.GetVisualStateGroups(element) is not { Count: > 0 } groups) return false;

        var any = false;
        foreach (VisualStateGroup group in groups)
        {
            foreach (VisualState state in group.States)
                if (state.Name == State)
                {
                    any = true;
                    break;
                }
            if (any) break;
        }

        return any;
    }

    public override void Execute(object? parameter)
    {
        if (!State.IsNotNullOrEmpty()) return;
        if (parameter is not FrameworkElement element) return;

        VisualStateManager.GoToState(element, State, UseTransition);
    }
}
