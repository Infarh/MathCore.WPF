using System.Windows.Input;
using System.Windows.Markup;

using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

[MarkupExtensionReturnType(typeof(TestWindow5ViewModel))]
public class TestWindow5ViewModel : ViewModel
{
    public TestWindow5ViewModel()
    {
        Enumerable.Range(1, 30).Select(i => new TestValueViewModel { Title = $"Value - {i}"}).AddTo(Values);
        Values.SelectedItem = Values.ElementAt(1);
    }

    public SelectableCollection<TestValueViewModel> Values { get; } = new();

    #region Command AddValueCommand : string - Добавить значение

    /// <summary>Добавить значение</summary>
    private ICommand? _AddValueCommand;

    /// <summary>Добавить значение</summary>
    public ICommand AddValueCommand => _AddValueCommand
        ??= Command.New<string>(p => Values.Add(new() { Title = p }), p => p is { Length: > 0 });

    #endregion
}

public class TestValueViewModel : TitledViewModel
{

}