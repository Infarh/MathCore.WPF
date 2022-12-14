using System.Windows.Input;
using System.Windows.Markup;

using MathCore.WPF.Commands;
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

    #region Value1 : int - Значение 1

    /// <summary>Значение 1</summary>
    private int _Value1 = 42;

    /// <summary>Значение 1</summary>
    public int Value1 { get => _Value1; set => Set(ref _Value1, value); }

    #endregion

    #region Value2 : string - Значение 2

    /// <summary>Значение 2</summary>
    private string _Value2 = "Test-1";

    /// <summary>Значение 2</summary>
    public string Value2 { get => _Value2; set => Set(ref _Value2, value); }

    #endregion

    public bool IsInDesignTime { get; } = IsDesignMode;

    #region Command TestAsyncCommand - Тестовая фоновая команда

    /// <summary>Тестовая фоновая команда</summary>
    private LambdaCommandAsync? _TestAsyncCommand;

    /// <summary>Тестовая фоновая команда</summary>
    public ICommand TestAsyncCommand => _TestAsyncCommand ??= Command.NewBackground(OnTestAsyncCommandExecuted);

    /// <summary>Логика выполнения - Тестовая фоновая команда</summary>
    private static async Task OnTestAsyncCommandExecuted()
    {
        var thread_id0 = Environment.CurrentManagedThreadId;

        await Task.Yield().ConfigureAwait(false);

        var thread_id1 = Environment.CurrentManagedThreadId;

        await Task.Yield().ConfigureAwaitWPF();

        var thread_id2 = Environment.CurrentManagedThreadId;

        await Task.Delay(100).ConfigureAwait(false);

        var thread_id3 = Environment.CurrentManagedThreadId;

        await Task.Delay(100).ConfigureAwaitWPF();

        var thread_id4 = Environment.CurrentManagedThreadId;

        await Task.Yield().ConfigureAwait(false);

        var thread_id5 = Environment.CurrentManagedThreadId;

        var task1 = Task.Delay(100);

        var thread_id6 = 0;

        var task2 = task1.OnSuccessWPF(() => thread_id6 = Environment.CurrentManagedThreadId);

        var thread_id7 = Environment.CurrentManagedThreadId;

        var thread_id8 = await task1.OnSuccessWPF(() => Environment.CurrentManagedThreadId);

        var thread_id9 = Environment.CurrentManagedThreadId;

        var thread_id10 = await Task.Run(
            async () =>
            {
                await Task.Delay(100).ConfigureAwait(false);
                return "Hello World!";
            })
           .OnSuccessWPF(s => (s, Environment.CurrentManagedThreadId));


        var thread_id_last = Environment.CurrentManagedThreadId;
    }

    #endregion
}