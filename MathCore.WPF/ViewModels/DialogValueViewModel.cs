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

        #region Result : TResult? - Результат диалога

        /// <summary>Результат диалога</summary>
        private TResult? _Result;

        /// <summary>Результат диалога</summary>
        public TResult? Result { get => _Result; set => Set(ref _Result, value); }

        #endregion
    }
}