using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для изменения размера элемента управления</summary>
public class Resize : Behavior<Control>
{
    #region AreaSize : double - Размер области

    /// <summary>DependencyProperty для свойства AreaSize</summary>
    public static readonly DependencyProperty AreaSizeProperty =
        DependencyProperty.Register(
            nameof(AreaSize),
            typeof(double),
            typeof(Resize),
            new(3d));

    /// <summary>Размер области захвата для изменения размера в пикселях</summary>
    public double AreaSize
    {
        get => (double)GetValue(AreaSizeProperty);
        set => SetValue(AreaSizeProperty, value);
    }

    #endregion

    #region TopResizing : bool - Изменение размера сверху

    public static readonly DependencyProperty TopResizingProperty =
        DependencyProperty.Register(
            nameof(TopResizing),
            typeof(bool),
            typeof(Resize),
            new(default(bool)));

    /// <summary>Изменение размера сверху</summary>
    public bool TopResizing
    {
        get => (bool)GetValue(TopResizingProperty);
        set => SetValue(TopResizingProperty, value);
    }

    #endregion

    #region BottomResizing : bool - Изменение размера снизу


    /// <summary>Изменение размера снизу</summary>
    public static readonly DependencyProperty BottomResizingProperty =
        DependencyProperty.Register(
            nameof(BottomResizing),
            typeof(bool),
            typeof(Resize),
            new(default(bool)));

    /// <summary>Изменение размера снизу</summary>
    public bool BottomResizing
    {
        get => (bool)GetValue(BottomResizingProperty);
        set => SetValue(BottomResizingProperty, value);
    }

    #endregion

    #region LeftResizing : bool - Изменение размера слева


    /// <summary>Изменение размера слева</summary>
    public static readonly DependencyProperty LeftResizingProperty =
        DependencyProperty.Register(
            nameof(LeftResizing),
            typeof(bool),
            typeof(Resize),
            new(default(bool)));

    /// <summary>Изменение размера слева</summary>
    public bool LeftResizing
    {
        get => (bool)GetValue(LeftResizingProperty);
        set => SetValue(LeftResizingProperty, value);
    }

    #endregion

    #region RightResizing : bool - Изменение размера справа

    /// <summary>Изменение размера справа</summary>
    public static readonly DependencyProperty RightResizingProperty =
        DependencyProperty.Register(
            nameof(RightResizing),
            typeof(bool),
            typeof(Resize),
            new(default(bool)));

    /// <summary>Изменение размера справа</summary>
    public bool RightResizing
    {
        get => (bool)GetValue(RightResizingProperty);
        set => SetValue(RightResizingProperty, value);
    }

    #endregion

    /// <summary>Флаг, указывающий нахождение мыши в области верхней границы</summary>
    private bool _InTop;
    /// <summary>Флаг, указывающий нахождение мыши в области нижней границы</summary>
    private bool _InBottom;
    /// <summary>Флаг, указывающий нахождение мыши в области левой границы</summary>
    private bool _InLeft;
    /// <summary>Флаг, указывающий нахождение мыши в области правой границы</summary>
    private bool _InRight;

    /// <summary>Флаг, указывающий нахождение мыши в любой из областей изменения размера</summary>
    private bool MouseInArea => _InLeft || _InRight || _InTop || _InBottom;

    /// <summary>Флаг, указывающий нахождение мыши в левом верхнем углу</summary>
    private bool MouseInLeftTopCorner => _InLeft && _InTop;
    /// <summary>Флаг, указывающий нахождение мыши в правом верхнем углу</summary>
    private bool MouseInRightTopCorner => _InRight && _InTop;
    /// <summary>Флаг, указывающий нахождение мыши в левом нижнем углу</summary>
    private bool MouseInLeftBottomCorner => _InLeft && _InBottom;
    /// <summary>Флаг, указывающий нахождение мыши в правом нижнем углу</summary>
    private bool MouseInRightBottomCorner => _InRight && _InBottom;

    /// <summary>Вызывается при присоединении поведения к элементу</summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseMove += OnMouseMove;
    }

    /// <summary>Вызывается при отсоединении поведения от элемента</summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.MouseMove -= OnMouseMove;
        Mouse.OverrideCursor = null;
    }

    /// <summary>Обработчик перемещения мыши для определения области изменения размера</summary>
    /// <param name="Sender">Источник события</param>
    /// <param name="E">Аргументы события</param>
    private void OnMouseMove(object Sender, MouseEventArgs E)
    {
        if (Sender is not Control control) return;
        var pos = E.GetPosition(control);

        var size = AreaSize;
        _InTop    = TopResizing && pos.Y <= size;
        _InLeft   = LeftResizing && pos.X <= size;
        _InBottom = BottomResizing && control.Height - pos.Y <= size;
        _InRight  = RightResizing && control.Width - pos.X <= size;


        Mouse.OverrideCursor = (Left: _InLeft, Top: _InTop, Right: _InRight, Bottom: _InBottom) switch
        {
            (Left: true, Top: true, Right: _, Bottom: _)       => Cursors.SizeNWSE,
            (Left: _, Top: _, Right: true, Bottom: true)       => Cursors.SizeNWSE,
            (Left: true, Top: _, Right: _, Bottom: true)       => Cursors.SizeNESW,
            (Left: _, Top: true, Right: true, Bottom: _)       => Cursors.SizeNESW,
            (Left: _, Top: true, Right: _, Bottom: _)          => Cursors.SizeNS,
            (Left: _, Top: _, Right: _, Bottom: true)          => Cursors.SizeNS,
            (Left: true, Top: _, Right: _, Bottom: _)          => Cursors.SizeWE,
            (Left: _, Top: _, Right: true, Bottom: _)          => Cursors.SizeWE,
            _                                                  => Cursors.Arrow
        };
    }
}