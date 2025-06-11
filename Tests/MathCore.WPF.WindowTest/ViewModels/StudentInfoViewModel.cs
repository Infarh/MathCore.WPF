using System.Diagnostics.CodeAnalysis;
using System.Windows.Data;
using System.Windows.Input;

using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

public class StudentInfoViewModel : ViewModel
{
    #region LastName : string - Фамилия

    /// <summary>Фамилия</summary>
    public string LastName { get; set => Set(ref field, value); } = "Фамилия-1";

    #endregion

    #region FirstName : string - Имя

    /// <summary>Имя</summary>
    public string FirstName { get; set => Set(ref field, value); } = "Имя-1";

    #endregion

    #region Patronymic : string - Отчество

    /// <summary>Отчество</summary>
    public string Patronymic { get; set => Set(ref field, value); } = "Отчество-1";

    #endregion

    #region Age : int - Отчество

    /// <summary>Отчество</summary>
    public int Age { get; set => Set(ref field, value, v => v is > 0 and < 123); } = 18;

    #endregion

    #region Group : string - Группа

    /// <summary>Группа</summary>
    public string Group { get; set => Set(ref field, value); } = "Группа-1";

    #endregion

    #region Command SaveCommand : BindingGroup - Принять изменения

    /// <summary>Принять изменения</summary>
    [field: AllowNull, MaybeNull]
    public ICommand SaveCommand => field ??= Command.New<BindingGroup>(
        p => p.CommitEdit(),
        p => p is not null);

    #endregion

    #region Command CancelCommand : BindingGroup - Отменить изменения

    /// <summary>Отменить изменения</summary>
    [field: AllowNull, MaybeNull]
    public ICommand CancelCommand => field ??= Command.New<BindingGroup>(
        p => p.CancelEdit(),
        p => p is { IsDirty: true });

    #endregion
}