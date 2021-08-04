using System;
using System.Collections.Generic;

using MathCore.Annotations;

namespace MathCore.WPF.ViewModels
{
    public partial class ViewModel
    {
        private PropertyChangedEventsSuppressor? _PropertyChangedEventsSuppressor;

        public sealed class PropertyChangedEventsSuppressor : IDisposable
        {
            private readonly Dictionary<string, DateTime> _RegistredEvents = new(10);
            private readonly ViewModel _Model;
            public TimeSpan Timeout { get; set; }


            internal PropertyChangedEventsSuppressor(ViewModel Model, TimeSpan Timeout)
            {
                _Model = Model ?? throw new ArgumentNullException(nameof(Model));
                this.Timeout = Timeout;
            }

            internal void RegisterEvent(string Event) => _RegistredEvents[Event] = DateTime.Now;

            public void Clear() => _RegistredEvents.Clear();

            public void Dispose()
            {
                if (ReferenceEquals(_Model._PropertyChangedEventsSuppressor, this))
                    _Model._PropertyChangedEventsSuppressor = null;

                if (Timeout == default)
                    foreach (var property_name in _RegistredEvents.Keys)
                        _Model.OnPropertyChanged(property_name);
                else
                    foreach (var (property_name, time) in _RegistredEvents)
                        if (DateTime.Now - time < Timeout)
                            _Model.OnPropertyChanged(property_name);
            }
        }

        public PropertyChangedEventsSuppressor SupressPropertyChanges(TimeSpan RegistrationTimeout = default)
        {
            if (_PropertyChangedEventsSuppressor is not { } suppressor)
                return _PropertyChangedEventsSuppressor = new PropertyChangedEventsSuppressor(this, RegistrationTimeout);
            suppressor.Timeout = RegistrationTimeout;
            return suppressor;
        }
    }
}
