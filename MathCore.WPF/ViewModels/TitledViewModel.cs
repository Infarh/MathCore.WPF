namespace MathCore.WPF.ViewModels
{
    public abstract class TitledViewModel : ViewModel
    {
        #region Title : string? - Заголовок

        /// <summary>Заголовок</summary>
        private string? _Title;

        /// <summary>Заголовок</summary>
        public string? Title { get => _Title; set => Set(ref _Title, value); }

        #endregion
    }
}
