using System.Collections.ObjectModel;
using System.Windows.Markup;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

[MarkupExtensionReturnType(typeof(TestWindow6ViewModel))]
public class TestWindow6ViewModel : TitledViewModel
{
    public TestWindow6ViewModel() : base("TestVM") => Enumerable.Range(1, 30).Select(i => new TestValueViewModel
    {
        Title = $"Value - {i}",
        Value = 15 - i
    }).AddTo(Values);

    public ObservableCollection<TestValueViewModel> Values { get; } = [];

    
}
