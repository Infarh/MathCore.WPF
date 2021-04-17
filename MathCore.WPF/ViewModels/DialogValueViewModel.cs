// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace MathCore.WPF.ViewModels
{
    public class DialogValueViewModel<TResult> : DialogViewModel
    {
        #region Caption : string? - Сообщение диалога

        /// <summary>Сообщение диалога</summary>
        private string? _Caption;

        /// <summary>Сообщение диалога</summary>
        public string? Caption { get => _Caption; set => Set(ref _Caption, value); }

        #endregion

        #region Value : TResult? - Результат диалога

        /// <summary>Результат диалога</summary>
        private TResult? _Value;

        /// <summary>Результат диалога</summary>
        public TResult? Value { get => _Value; set => Set(ref _Value, value); }

        #endregion
    }
}