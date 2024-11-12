// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace MathCore.WPF.ViewModels;

/// <summary>Базовая модель представления диалога с результатом.</summary>
/// <typeparam name="TResult">Тип результата диалога.</typeparam>
public class DialogValueViewModel<TResult> : DialogViewModel
{
    #region Caption : string? - Сообщение диалога

    /// <summary>Сообщение диалога.</summary>
    private string? _Caption;

    /// <summary>Возвращает или устанавливает сообщение диалога.</summary>
    public string? Caption
    {
        get => _Caption;
        set => Set(ref _Caption, value);
    }

    #endregion

    #region Value : TResult? - Результат диалога

    /// <summary>Результат диалога.</summary>
    private TResult? _Value;

    /// <summary>Возвращает или устанавливает результат диалога.</summary>
    public TResult? Value
    {
        get => _Value;
        set => Set(ref _Value, value);
    }

    #endregion
}