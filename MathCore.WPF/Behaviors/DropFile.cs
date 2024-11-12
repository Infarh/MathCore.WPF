using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для элементов, поддерживающее получение файла через Drag-and-Drop.</summary>
public class DropFile : Behavior<FrameworkElement>
{
    #region DropFileCommand : ICommand - Команда получения файла

    /// <summary>Команда получения файла.</summary>
    public static readonly DependencyProperty DropFileCommandProperty =
        DependencyProperty.Register(
            nameof(DropFileCommand),
            typeof(ICommand),
            typeof(DropFile),
            new(default(ICommand)));

    /// <summary>Команда получения файла.</summary>
    /// <remarks>Эта команда будет вызвана при успешном получении файла.</remarks>
    [Description("Команда получения файла")]
    public ICommand DropFileCommand
    {
        get => (ICommand)GetValue(DropFileCommandProperty);
        set => SetValue(DropFileCommandProperty, value);
    }

    #endregion

    /// <summary>Вызывается при присоединении поведения к элементу.</summary>
    protected override void OnAttached()
    {
        base.OnAttached();

        // Получаем элемент, к которому присоединено поведение
        var element = AssociatedObject;

        // Разрешаем получение файла через Drag-and-Drop
        element.AllowDrop = true;

        // Если элемент является контролом и не имеет заднего фона, устанавливаем прозрачный фон
        if (element is Control { Background: null } control)
            control.Background = Brushes.Transparent;

        // Подписываемся на события DragEnter и Drop
        element.DragEnter += OnDragEnter;
        element.Drop += OnDrop;
    }

    /// <summary>Вызывается при отсоединении поведения от элемента.</summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();

        // Получаем элемент, от которого отсоединено поведение
        var element = AssociatedObject;

        // Отменяем подписку на события DragEnter и Drop
        element.DragEnter -= OnDragEnter;
        element.Drop -= OnDrop;
    }

    /// <summary>Обработчик события DragEnter.</summary>
    /// <param name="Sender">Источник события.</param>
    /// <param name="E">Аргументы события.</param>
    private static void OnDragEnter(object Sender, DragEventArgs E)
    {
        // Проверяем, поддерживается ли формат данных DataFormats.FileDrop
        if (!E.Data.GetDataPresent(DataFormats.FileDrop)) return;

        // Получаем массив файлов, которые были перетащены
        var files = (string[])E.Data.GetData(DataFormats.FileDrop)!;

        // Проверяем, существуют ли все файлы
        if (!files.Any(File.Exists)) return;

        // Указываем, что данные были обработаны
        E.Handled = true;

        // Указываем, что файлы будут скопированы
        E.Effects = DragDropEffects.Copy;
    }

    /// <summary>Обработчик события Drop.</summary>
    /// <param name="Sender">Источник события.</param>
    /// <param name="E">Аргументы события.</param>
    private void OnDrop(object Sender, DragEventArgs E)
    {
        // Проверяем, поддерживается ли формат данных DataFormats.FileDrop
        if (!E.Data.GetDataPresent(DataFormats.FileDrop) || DropFileCommand is not { } command) return;

        // Получаем массив файлов, которые были перетащены
        var files = (string[])E.Data.GetData(DataFormats.FileDrop)!;

        // Выполняем команду получения файла для каждого существующего файла
        foreach (var path in files)
            if (new FileInfo(path) is { Exists: true } file && command.CanExecute(file))
                command.Execute(file);
    }
}
