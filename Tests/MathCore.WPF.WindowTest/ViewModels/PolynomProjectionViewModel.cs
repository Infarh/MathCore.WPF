using System.Windows.Markup;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

[MarkupExtensionReturnType(typeof(PolynomProjectionViewModel))]
public class PolynomProjectionViewModel : TitledViewModel
{
    public PolynomProjectionViewModel() => Title = "Проверка";


}