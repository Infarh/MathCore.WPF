using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows.Markup;

using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

[MarkupExtensionReturnType(typeof(TestWindow5ViewModel))]
public class TestWindow5ViewModel : ViewModel
{
    public TestWindow5ViewModel()
    {
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
            var index = path.LastIndexOfAny(new[] { '\\', '/' });
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
        catch (Exception ex)
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


}

public class TestValueViewModel : TitledViewModel
{

}