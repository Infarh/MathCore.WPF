using System;
using System.Windows.Markup;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

[MarkupExtensionReturnType(typeof(TestWindow1ViewModel))]
public class TestWindow1ViewModel : TitledViewModel
{
    public TestWindow1ViewModel() => Title = "Тестовое окно №1";

    #region ElementWidth : double - Ширина элемента

    /// <summary>Ширина элемента</summary>
    private double _ElementWidth;

    /// <summary>Ширина элемента</summary>
    public double ElementWidth
    {
        get => _ElementWidth;
        set => Set(ref _ElementWidth, value);
    }

    #endregion

    #region ElementHeight : double - Высота элемента

    /// <summary>Высота элемента</summary>
    private double _ElementHeight;

    /// <summary>Высота элемента</summary>
    public double ElementHeight
    {
        get => _ElementHeight;
        set => Set(ref _ElementHeight, value);
    }

    #endregion

    [DependencyOn(nameof(ElementWidth))]
    [DependencyOn(nameof(ElementHeight))]
    public string TextSize => $"({_ElementWidth:0.00} x {_ElementHeight:0.00})";
}