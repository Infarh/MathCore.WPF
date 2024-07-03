using System.Windows.Markup;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

[MarkupExtensionReturnType(typeof(TestWindow4ViewModel))]
public class TestWindow4ViewModel() : TitledViewModel("Главное окно 4")
{
    #region ValueInt32 : int - Целочисленное значение

    /// <summary>Целочисленное значение</summary>
    private int _ValueInt32 = 32;

    /// <summary>Целочисленное значение</summary>
    public int ValueInt32 { get => _ValueInt32; set => Set(ref _ValueInt32, value); }

    #endregion

    #region ValueDouble : double - Вещественное значение

    /// <summary>Вещественное значение</summary>
    private double _ValueDouble = 3.1415926535;

    /// <summary>Вещественное значение</summary>
    public double ValueDouble { get => _ValueDouble; set => Set(ref _ValueDouble, value); }

    #endregion
}
