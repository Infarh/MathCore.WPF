using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для окна, позволяющее задать анимацию закрытия и установить DialogResult.</summary>
public class CloseBehavior : Behavior<Window>
{
    #region CloseWithDialogResultProperty : bool - Признак, при установке которого в истину, окно будет закрыто

    /// <summary>DependencyProperty для свойства CloseWithDialogResult.</summary>
    public static readonly DependencyProperty CloseWithDialogResultProperty =
        DependencyProperty.Register(
            nameof(CloseWithDialogResult),
            typeof(bool),
            typeof(CloseBehavior),
            new(default(bool), OnCloseTriggerChanged));

    /// <summary>Обработчик изменения свойства CloseWithDialogResult.</summary>
    /// <param name="d">Объект, на котором произошло изменение свойства.</param>
    /// <param name="e">Аргументы изменения свойства.</param>
    private static void OnCloseTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CloseBehavior behavior || e.NewValue is not bool dialog_result) return;
        var window = behavior.AssociatedObject;
        window.DialogResult = dialog_result;
        if (behavior._CloseInProgress) return;
        window.Close();
    }

    /// <summary>Признак, при установке которого в истину, окно будет закрыто.</summary>
    public bool CloseWithDialogResult
    {
        get => (bool)GetValue(CloseWithDialogResultProperty);
        set => SetValue(CloseWithDialogResultProperty, value);
    }

    #endregion

    #region Storyboard : Storyboard

    /// <summary>DependencyProperty для свойства Storyboard.</summary>
    public static readonly DependencyProperty StoryboardProperty =
        DependencyProperty.Register(
            "StoryboardShadow", //nameof(Storyboard), 
            typeof(Storyboard),
            typeof(CloseBehavior),
            new(default(Storyboard)));

    /// <summary>Анимация закрытия окна.</summary>
    public Storyboard? Storyboard
    {
        get => (Storyboard)GetValue(StoryboardProperty);
        set => SetValue(StoryboardProperty, value);
    }

    #endregion

    /// <summary>Вызывается при присоединении поведения к окну.</summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Closing += OnWindowClosing;
    }

    /// <summary>Вызывается при отсоединении поведения от окна.</summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Closing -= OnWindowClosing;
    }

    /// <summary>Флаг, указывающий, что закрытие окна уже началось.</summary>
    private bool _CloseInProgress;

    /// <summary>Обработчик события Closing окна.</summary>
    /// <param name="sender">Источник события.</param>
    /// <param name="e">Аргументы события.</param>
    private void OnWindowClosing(object sender, CancelEventArgs e)
    {
        if (Storyboard is null) return;
        _CloseInProgress = true;
        e.Cancel = true;
        AssociatedObject.Closing -= OnWindowClosing;

        Storyboard.Completed += OnStoryboardOnCompleted;
        Storyboard.Begin(AssociatedObject);
    }

    /// <summary>Обработчик события завершения анимации Storyboard.</summary>
    /// <param name="sender">Источник события.</param>
    /// <param name="e">Аргументы события.</param>
    private void OnStoryboardOnCompleted(object? sender, EventArgs e)
    {
        Storyboard = null;
        if (ReadLocalValue(CloseWithDialogResultProperty) != DependencyProperty.UnsetValue)
            AssociatedObject.DialogResult = CloseWithDialogResult;
        AssociatedObject.Close();
    }
}