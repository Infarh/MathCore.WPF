namespace MathCore.WPF.ViewModels;

public partial class ViewModel
{
    private PropertyChangedEventsSuppressor? _PropertyChangedEventsSuppressor;

    /// <summary>Класс, позволяющий подавить события изменения свойств в модели представления.</summary>
    public sealed class PropertyChangedEventsSuppressor : IDisposable
    {
        /// <summary>Словарь, хранящий имена свойств и время их последнего изменения.</summary>
        private readonly Dictionary<string, DateTime> _RegisteredEvents = new(10);

        /// <summary>Модель представления, для которой подавляются события изменения свойств.</summary>
        private readonly ViewModel _Model;

        /// <summary>Время, в течение которого подавляются события изменения свойств.</summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>Инициализирует новый экземпляр класса <see cref="PropertyChangedEventsSuppressor"/>.</summary>
        /// <param name="Model">Модель представления, для которой подавляются события изменения свойств.</param>
        /// <param name="Timeout">Время, в течение которого подавляются события изменения свойств.</param>
        internal PropertyChangedEventsSuppressor(ViewModel Model, TimeSpan Timeout)
        {
            _Model = Model ?? throw new ArgumentNullException(nameof(Model));
            this.Timeout = Timeout;
        }

        /// <summary>Регистрирует событие изменения свойства.</summary>
        /// <param name="Event">Имя свойства, которое изменилось.</param>
        internal void RegisterEvent(string Event) => _RegisteredEvents[Event] = DateTime.Now;

        /// <summary>Очищает список зарегистрированных событий изменения свойств.</summary>
        public void Clear() => _RegisteredEvents.Clear();

        /// <summary>Освобождает ресурсы, используемые классом <see cref="PropertyChangedEventsSuppressor"/>.</summary>
        public void Dispose()
        {
            // Если этот экземпляр класса является текущим подавителем событий в модели представления,
            // то сбрасываем ссылку на этот экземпляр в модели представления.
            if (ReferenceEquals(_Model._PropertyChangedEventsSuppressor, this))
                _Model._PropertyChangedEventsSuppressor = null;

            // Если время подавления событий не задано, то генерируем события изменения свойств для всех зарегистрированных свойств.
            if (Timeout == default)
                foreach (var property_name in _RegisteredEvents.Keys)
                    _Model.OnPropertyChanged(property_name);
            // Если время подавления событий задано, то генерируем события изменения свойств только для тех свойств,
            // которые были изменены в течение этого времени.
            else
                foreach (var (property_name, time) in _RegisteredEvents)
                    if (DateTime.Now - time < Timeout)
                        _Model.OnPropertyChanged(property_name);
        }
    }

    /// <summary>
    /// Возвращает или создает объект, подавляющий события изменения свойств в течение указанного таймаута.
    /// </summary>
    /// <param name="RegistrationTimeout">Время, в течение которого будут подавляться события изменения свойств.</param>
    /// <returns>Объект, подавляющий события изменения свойств.</returns>
    public PropertyChangedEventsSuppressor SuppressPropertyChanges(TimeSpan RegistrationTimeout = default)
    {
        if (_PropertyChangedEventsSuppressor is not { } suppressor)
            return _PropertyChangedEventsSuppressor = new(this, RegistrationTimeout);
        suppressor.Timeout = RegistrationTimeout;
        return suppressor;
    }
}