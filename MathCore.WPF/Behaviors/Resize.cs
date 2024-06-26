﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Behaviors;

public class Resize : Behavior<Control>
{
    #region AreaSize : double - Размер области

    public static readonly DependencyProperty AreaSizeProperty =
        DependencyProperty.Register(
            nameof(AreaSize),
            typeof(double),
            typeof(Resize),
            new(3d));

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

    private bool _InTop;
    private bool _InBottom;
    private bool _InLeft;
    private bool _InRight;

    private bool MouseInArea => _InLeft || _InRight || _InTop || _InBottom;

    private bool MouseInLeftTopCorner => _InLeft && _InTop;
    private bool MouseInRightTopCorner => _InRight && _InTop;
    private bool MouseInLeftBottomCorner => _InLeft && _InBottom;
    private bool MouseInRightBottomCorner => _InRight && _InBottom;

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseDown += OnMouseDown;
        AssociatedObject.MouseUp   += OnMouseUp;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.MouseMove -= OnMouseMove;

    }

    private void OnMouseUp(object Sender, MouseButtonEventArgs E)
    {

    }

    private void OnMouseDown(object Sender, MouseButtonEventArgs E)
    {

    }

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
            (Left: true, Top: true, Right: _, Bottom   : _)    => Mouse.OverrideCursor = Cursors.ScrollWE,
            (Left: _, Top   : _, Right   : true, Bottom: true) => Mouse.OverrideCursor = Cursors.ScrollWE,
            (Left: _, Top   : true, Right: _, Bottom   : _)    => Mouse.OverrideCursor = Cursors.ScrollNS,
            (Left: _, Top   : _, Right   : _, Bottom   : true) => Mouse.OverrideCursor = Cursors.ScrollNS,
            (Left: true, Top: _, Right   : _, Bottom   : _)    => Mouse.OverrideCursor = Cursors.ScrollWE,
            (Left: _, Top   : _, Right   : true, Bottom: _)    => Mouse.OverrideCursor = Cursors.ScrollWE,
            _                                                  => Cursors.Arrow
        };

        //if ((_InLeft && _InTop) || (_InRight && _InBottom)) Mouse.OverrideCursor = Cursors.ScrollWE;
        //else if ((_InRight && _InTop) || (_InLeft && _InBottom)) Mouse.OverrideCursor = Cursors.ScrollNE;
        //else if (_InTop || _InBottom) Mouse.OverrideCursor = Cursors.ScrollNS;
        //else if (_InLeft || _InRight) Mouse.OverrideCursor = Cursors.ScrollWE;
        //else Mouse.OverrideCursor = Cursors.Arrow;
    }
}