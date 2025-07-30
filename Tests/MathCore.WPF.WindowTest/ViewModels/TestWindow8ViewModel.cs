using System.Windows.Markup;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

[MarkupExtensionReturnType(typeof(TestWindow8ViewModel))]
internal class TestWindow8ViewModel() : TitledViewModel("Тестовое окно 8")
{

}
