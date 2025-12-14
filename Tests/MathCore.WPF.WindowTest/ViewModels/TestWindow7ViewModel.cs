using System.Windows.Markup;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

[MarkupExtensionReturnType(typeof(TestWindow7ViewModel))]
public class TestWindow7ViewModel() : TitledViewModel("Поток событий")
{
    #region Events : SelectableCollection<TestEventViewModel> - События

    /// <summary>События</summary>
    public SelectableCollection<TestEventViewModel> Events { get; set => Set(ref field, value); } = [.. GenerateEvents()];

    private static IEnumerable<TestEventViewModel> GenerateEvents()
    {
        for (var i = 0; i < 10; i++)
            yield return new($"Event-{i + 1}")
            {
                Time = DateTime.Now.AddMinutes(i * -10),
                Description = 
                    $"""
                    Event {i + 1} description
                    123
                      32112312
                      123123
                    """
            }; 
    }

    #endregion
}

public class TestEventViewModel(string EventName) : TitledViewModel(EventName)
{
    #region Time : DateTime - Время

    /// <summary>Время</summary>
    public DateTime Time { get; set => Set(ref field, value); }

    #endregion

    #region Description : string - Описание

    /// <summary>Описание</summary>
    public string Description { get; set => Set(ref field, value); } = null!;

    #endregion
}