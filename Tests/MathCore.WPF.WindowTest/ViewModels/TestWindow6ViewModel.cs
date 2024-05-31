using System.Collections.ObjectModel;
using System.Windows.Markup;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

[MarkupExtensionReturnType(typeof(TestWindow6ViewModel))]
public class TestWindow6ViewModel() : TitledViewModel("TestVM")
{
    #region Value : double - Значение

    /// <summary>Значение</summary>
    private double _Value = 5;

    /// <summary>Значение</summary>
    public double Value { get => _Value; set => Set(ref _Value, value); }

    #endregion
}
