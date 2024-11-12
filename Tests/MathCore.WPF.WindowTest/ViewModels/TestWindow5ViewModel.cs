using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows.Markup;

using MathCore.MathParser;
using MathCore.MathParser.ExpressionTrees.Nodes;
using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;

using Microsoft.Extensions.Logging;

namespace MathCore.WPF.WindowTest.ViewModels;

[MarkupExtensionReturnType(typeof(TestWindow5ViewModel))]
public class TestWindow5ViewModel : ViewModel
{
    private readonly ILogger<TestWindow5ViewModel> _Logger;

    public TestWindow5ViewModel(ILogger<TestWindow5ViewModel> logger)
    {
        _Logger = logger;

        Enumerable.Range(1, 30).Select(i => new TestValueViewModel { Title = $"Value - {i}" }).AddTo(Values);
        Values.SelectedItem = Values.ElementAt(1);
    }

    public SelectableCollection<TestValueViewModel> Values { get; } = [];

    #region Command AddValueCommand : string - Добавить значение

    /// <summary>Добавить значение</summary>
    private ICommand? _AddValueCommand;

    /// <summary>Добавить значение</summary>
    public ICommand AddValueCommand => _AddValueCommand
        ??= Command.New<string>(p => Values.Add(new() { Title = p }), p => p is { Length: > 0 });

    #endregion



    #region property DirPath : string - Путь к каталогу

    /// <Summary>Путь к каталогу</Summary>
    private string _DirPath;

    /// <Summary>Путь к каталогу</Summary>
    public string DirPath
    {
        get => _DirPath;
        set
        {
            if (!SetValue(ref _DirPath, value)) return;
            _ = UpdateCatalogPaths(value);
        }
    }

    private long _LastUpdateCatalogPathsTime;


    private async ValueTask UpdateCatalogPaths(string path)
    {
        if(path is not { Length: > 0 })
        {
            CatalogsPaths = null;
            return;
        }

        if (path is [_, ':'])
            return;

        if (!Directory.Exists(path))
        {
            var index = path.LastIndexOfAny(['\\', '/']);
            if (index < 0) return;

            await UpdateCatalogPaths(path[..index]);
        }

        var now = Environment.TickCount64;
        _LastUpdateCatalogPathsTime = now;

        await Task.Delay(300).ConfigureAwait(false);
        if (_LastUpdateCatalogPathsTime != now) return;

        Debug.Write("Path:");
        Debug.WriteLine(path);

        try
        {
            CatalogsPaths = Directory.GetDirectories(path);

            if (!(CatalogsPaths?.Count() > 0)) return;

            Debug.Write("    ");
            Debug.WriteLine(CatalogsPaths.JoinStrings("\r\n    "));
        }
        catch (Exception)
        {
            throw;
        }
    }

    #endregion


    #region property CatalogsPaths : IEnumerable<string> - Возможные варианты

    /// <Summary>Возможные варианты</Summary>
    private IEnumerable<string> _CatalogsPaths;

    /// <Summary>Возможные варианты</Summary>
    public IEnumerable<string> CatalogsPaths { get => _CatalogsPaths; set => Set(ref _CatalogsPaths, value); }

    #endregion

    #region TestExpr : string - Математическое выражение

    private readonly ExpressionParser _Parser = new();

    /// <summary>Математическое выражение</summary>
    private string _TestExpr;

    /// <summary>Математическое выражение</summary>
    public string TestExpr { get => _TestExpr; set => SetValue(ref _TestExpr, value).Then(ParseExpressionAsync); }

    private int _Timeout = 300;
    private long _LastExprParseTime;
    private async void ParseExpressionAsync()
    {
        if (_TestExpr is not { Length: > 0 } expr_str)
        {
            ExpressionParseException = null;
            Expression = null;
            OnPropertyChanged(nameof(Expression));
            return;
        }

        var now = Environment.TickCount64;
        _LastExprParseTime = now;

        var timeout = _Timeout;

        if (timeout > 0)
            await Task.Delay(timeout * 2).ConfigureAwait(false);
        else
            await Task.Yield().ConfigureAwait(false);
                
        if(_LastExprParseTime != now || expr_str != _TestExpr)
            return;

        try
        {
            var expr = _Parser.Parse(expr_str);

            var end_time = Environment.TickCount64;
            var time_delta = TimeSpan.FromTicks(end_time - now).TotalMilliseconds;

            _Timeout = Math.Max(0, (int)(_Timeout + (time_delta - _Timeout) / 10));

            _Logger.LogInformation("Expr parsed ({timeout}ms): {expr}", _Timeout, expr);

            ExpressionParseException = null;
            Expression = expr;
            OnPropertyChanged(nameof(Expression));
        }
        catch (Exception e)
        {
            _Logger.LogWarning("Expr parse error: \"{expr}\" - {errtype}:{error}", expr_str, e.GetType().Name, e.Message);

            ExpressionParseException = null;
            OnPropertyChanged(nameof(ExpressionParseException));
        }
    }

    #endregion

    public MathExpression? Expression { get; private set; }

    [DependencyOn(nameof(Expression))]
    public IEnumerable<ExpressionTreeNode> ExpressionTree => [Expression?.Tree.Root];

    [DependencyOn(nameof(Expression))]
    public Exception ExpressionParseException { get; private set; }

}

public class TestValueViewModel : TitledViewModel
{
    #region Value : int - Значение

    /// <summary>Значение</summary>
    private int _Value = 5;

    /// <summary>Значение</summary>
    public int Value { get => _Value; set => Set(ref _Value, value); }

    #endregion
}