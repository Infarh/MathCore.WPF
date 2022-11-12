using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using MathCore.WPF.Converters;
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace MathCore.WPF.Values;

public class ScaleValue : Freezable
{
    private bool _IsValueLast;
    private readonly Binding _ValueBinding;
    private readonly BindingExpressionBase _ValueBindingExpression;

    #region ScreenLength : double - Размер экрана

    /// <summary>Размер экрана</summary>
    public static readonly DependencyProperty ScreenLengthProperty =
        DependencyProperty.Register(
            nameof(ScreenLength),
            typeof(double),
            typeof(ScaleValue),
            new PropertyMetadata(default(double), (d, _) => ((ScaleValue)d).Update()));

    /// <summary>Размер экрана</summary>
    [Bindable(true)]
    public double ScreenLength
    {
        get => (double)GetValue(ScreenLengthProperty);
        set => SetValue(ScreenLengthProperty, value);
    }

    #endregion

    #region ValueMin : double - Отображаемый минимум

    /// <summary>Отображаемый минимум</summary>
    public static readonly DependencyProperty ValueMinProperty =
        DependencyProperty.Register(
            nameof(ValueMin),
            typeof(double),
            typeof(ScaleValue),
            new PropertyMetadata(default(double), (d, _) => ((ScaleValue)d).Update()));

    /// <summary>Отображаемый минимум</summary>
    [Bindable(true)]
    public double ValueMin
    {
        get => (double)GetValue(ValueMinProperty);
        set => SetValue(ValueMinProperty, value);
    }

    #endregion

    #region ValueMax : double - Отображаемый максимум

    /// <summary>Отображаемый максимум</summary>
    public static readonly DependencyProperty ValueMaxProperty =
        DependencyProperty.Register(
            nameof(ValueMax),
            typeof(double),
            typeof(ScaleValue),
            new PropertyMetadata(default(double), (d, _) => ((ScaleValue)d).Update()));

    /// <summary>Отображаемый максимум</summary>
    [Bindable(true)]
    public double ValueMax
    {
        get => (double)GetValue(ValueMaxProperty);
        set => SetValue(ValueMaxProperty, value);
    }

    #endregion

    #region ValueOffset : double - Смещение значения

    /// <summary>Смещение значения</summary>
    public static readonly DependencyProperty ValueOffsetProperty =
        DependencyProperty.Register(
            nameof(ValueOffset),
            typeof(double),
            typeof(ScaleValue),
            new PropertyMetadata(default(double), (d, _) => ((ScaleValue)d).Update()));

    /// <summary>Смещение значения</summary>
    [Bindable(true)]
    public double ValueOffset
    {
        get => (double)GetValue(ValueOffsetProperty);
        set => SetValue(ValueOffsetProperty, value);
    }

    #endregion

    #region ScreenOffset : double - Экранное смещение

    /// <summary>Экранное смещение</summary>
    public static readonly DependencyProperty ScreenOffsetProperty =
        DependencyProperty.Register(
            nameof(ScreenOffset),
            typeof(double),
            typeof(ScaleValue),
            new PropertyMetadata(default(double), (d, _) => ((ScaleValue)d).Update()));

    /// <summary>Экранное смещение</summary>
    [Bindable(true)]
    public double ScreenOffset
    {
        get => (double)GetValue(ScreenOffsetProperty);
        set => SetValue(ScreenOffsetProperty, value);
    }

    #endregion

    #region Value : double - Значение

    /// <summary>Значение</summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(double),
            typeof(ScaleValue),
            new FrameworkPropertyMetadata(
                default(double),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (d, _) => ((ScaleValue)d)._IsValueLast = true));

    /// <summary>Значение</summary>
    [Bindable(true)]
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    #endregion

    #region ScreenValue : double - Экранное значение

    /// <summary>Экранное значение</summary>
    public static readonly DependencyProperty ScreenValueProperty =
        DependencyProperty.Register(
            nameof(ScreenValue),
            typeof(double),
            typeof(ScaleValue),
            new FrameworkPropertyMetadata(
                default(double),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (d, _) => ((ScaleValue)d)._IsValueLast = false));

    /// <summary>Экранное значение</summary>
    [Bindable(true)]
    public double ScreenValue
    {
        get => (double)GetValue(ScreenValueProperty);
        set => SetValue(ScreenValueProperty, value);
    }

    #endregion

    public ScaleValue()
    {
        _ValueBinding = new Binding(nameof(Value))
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.Self),
            Converter      = new LambdaConverter(Convert, ConvertBack),
            Mode           = BindingMode.TwoWay
        };
        _ValueBindingExpression = BindingOperations.SetBinding(this, ScreenValueProperty, _ValueBinding);
        //BindingOperations.GetBindingExpression(this, ScreenValueProperty)?.UpdateTarget();
    }

    private void Update()
    {
        if (_IsValueLast)
            _ValueBindingExpression.UpdateSource();
        else
            _ValueBindingExpression.UpdateTarget();
    }

    #region IValueConverter

    private object? Convert(object? v, Type? t, object? p, CultureInfo? c)
    {
        if (v is null) return null;
        var screen_length = ScreenLength;
        if (screen_length.Equals(0d)) return double.NaN;
        var value_min   = ValueMin;
        var value_max   = ValueMax;
        var value_delta = value_max - value_min;
        var value       = (double)System.Convert.ChangeType(v, TypeCode.Double);

        var screen_value = (value - value_min + ValueOffset) * screen_length / value_delta + ScreenOffset;

        return screen_value;
    }

    private object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c)
    {
        if (v is null) return null;
        var screen_length = ScreenLength;
        if (screen_length.Equals(0d)) return double.NaN;
        var value_min    = ValueMin;
        var value_max    = ValueMax;
        var value_delta  = value_max - value_min;
        var screen_value = (double)System.Convert.ChangeType(v, TypeCode.Double);

        var value = (screen_value - ScreenOffset) * value_delta / screen_length + value_min - ValueOffset;

        return value;
    }

    #endregion

    #region Freezable

    protected override Freezable CreateInstanceCore() =>
        new ScaleValue
        {
            ScreenLength = ScreenLength,
            ValueMax     = ValueMax,
            ValueMin     = ValueMin
        };

    #endregion
}