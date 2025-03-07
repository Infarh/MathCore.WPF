using System.Windows.Data;

namespace MathCore.WPF.Commands;

/// <summary>Команда для подтверждения изменений в BindingGroup, если они были изменены</summary>
public class BindingGroupCommit : Command
{
    public override bool CanExecute(object? parameter) => parameter is BindingGroup { IsDirty: true };

    public override void Execute(object? parameter) => (parameter as BindingGroup).CommitEdit();
}
