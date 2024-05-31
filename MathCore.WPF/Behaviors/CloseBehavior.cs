using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Behaviors;

public class CloseBehavior : Behavior<Window>
{
    #region CloseWithDialogResultProperty : bool - Признак, при установке которого в истину, окно будет закрыто

    /// <summary>Признак, при установке которого в истину, окно будет закрыто</summary>
    public static readonly DependencyProperty CloseWithDialogResultProperty =
        DependencyProperty.Register(
            nameof(CloseWithDialogResult),
            typeof(bool),
            typeof(CloseBehavior),
            new(default(bool), OnCloseTriggerChanged));

    private static void OnCloseTriggerChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        if (D is not CloseBehavior behavior || E.NewValue is not bool dialog_result) return;
        var window = behavior.AssociatedObject;
        window.DialogResult = dialog_result;
        if (behavior._CloseInProgress) return;
        window.Close();
    }

    /// <summary>Признак, при установке которого в истину, окно будет закрыто</summary>
    public bool CloseWithDialogResult
    {
        get => (bool)GetValue(CloseWithDialogResultProperty);
        set => SetValue(CloseWithDialogResultProperty, value);
    }

    #endregion

    #region Storyboard : Storyboard

    public static readonly DependencyProperty StoryboardProperty =
        DependencyProperty.Register(
            "StoryboardShadow", //nameof(Storyboard), 
            typeof(Storyboard),
            typeof(CloseBehavior),
            new(default(Storyboard)));

    public Storyboard? Storyboard
    {
        get => (Storyboard)GetValue(StoryboardProperty);
        set => SetValue(StoryboardProperty, value);
    } 

    #endregion

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Closing += OnWindowClosing;
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Closing -= OnWindowClosing;
    }

    private bool _CloseInProgress;

    private void OnWindowClosing(object sender, CancelEventArgs e)
    {
        if (Storyboard is null) return;
        _CloseInProgress         =  true;
        e.Cancel                 =  true;
        AssociatedObject.Closing -= OnWindowClosing;

        Storyboard.Completed += OnStoryboardOnCompleted;
        Storyboard.Begin(AssociatedObject);
    }

    private void OnStoryboardOnCompleted(object? sender, EventArgs e)
    {
        Storyboard = null;
        if (ReadLocalValue(CloseWithDialogResultProperty) != DependencyProperty.UnsetValue)
            AssociatedObject.DialogResult = CloseWithDialogResult;
        AssociatedObject.Close();

    }
}