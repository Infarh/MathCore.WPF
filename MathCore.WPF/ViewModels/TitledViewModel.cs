// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace MathCore.WPF.ViewModels;

/// <summary>Модель-представления, обладающая заголовком</summary>
public abstract class TitledViewModel : ViewModel
{
    #region Title : string? - Заголовок

    /// <summary>Заголовок</summary>
    private string? _Title;

    /// <summary>Заголовок</summary>
    public string? Title { get => _Title; set => Set(ref _Title, value); }

    #endregion

    protected TitledViewModel() { }

    protected TitledViewModel(string Title) => _Title = Title;
}