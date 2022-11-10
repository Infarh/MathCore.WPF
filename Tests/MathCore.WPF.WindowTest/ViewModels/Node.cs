using System.Windows;
using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

class Node : ViewModel
{
    #region Position : Point - Положение

    /// <summary>Положение</summary>
    private Point _Position;

    /// <summary>Положение</summary>
    public Point Position { get => _Position; set => Set(ref _Position, value); }

    #endregion

    #region Radius : double - Размер

    /// <summary>Размер</summary>
    private double _Radius;

    /// <summary>Размер</summary>
    public double Radius { get => _Radius; set => Set(ref _Radius, value); }

    #endregion
}