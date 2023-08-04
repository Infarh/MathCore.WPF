using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors;

public class DropFile : Behavior<FrameworkElement>
{
    #region DropFileCommand : ICommand - Команда получения файла

    /// <summary>Команда получения файла</summary>
    public static readonly DependencyProperty DropFileCommandProperty =
        DependencyProperty.Register(
            nameof(DropFileCommand),
            typeof(ICommand),
            typeof(DropFile),
            new PropertyMetadata(default(ICommand)));

    /// <summary>Команда получения файла</summary>
    //[Category("")]
    [Description("$summary$")]
    public ICommand DropFileCommand
    {
        get => (ICommand)GetValue(DropFileCommandProperty);
        set => SetValue(DropFileCommandProperty, value);
    }

    #endregion

    protected override void OnAttached()
    {
        base.OnAttached();

        var element = AssociatedObject;

        element.AllowDrop = true;
        if (element is Control { Background: null } control)
            control.Background = Brushes.Transparent;

        element.DragEnter += OnDragEnter;
        element.Drop += OnDrop;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        var element = AssociatedObject;
        element.DragEnter -= OnDragEnter;
        element.Drop -= OnDrop;
    }

    private void OnDragEnter(object Sender, DragEventArgs E)
    {
        if (!E.Data.GetDataPresent(DataFormats.FileDrop)) return;
        if (!((string[])E.Data.GetData(DataFormats.FileDrop)!).Any(File.Exists)) return;

        E.Handled = true;
        E.Effects = DragDropEffects.Copy;
    }

    private void OnDrop(object Sender, DragEventArgs E)
    {
        if (!E.Data.GetDataPresent(DataFormats.FileDrop) || DropFileCommand is not { } command) return;

        foreach (var path in (string[])E.Data.GetData(DataFormats.FileDrop)!)
            if (new FileInfo(path) is { Exists: true } file && command.CanExecute(file))
                command.Execute(file);
    }
}

