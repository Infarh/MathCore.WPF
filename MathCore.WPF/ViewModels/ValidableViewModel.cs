using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Markup;

using MathCore.Extensions.Expressions;
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace MathCore.WPF.ViewModels;

/// <summary>Модель-представления, обеспечивающая возможность валидации данных на основе атрибутов</summary>
[MarkupExtensionReturnType(typeof(ValidableViewModel))]
public abstract class ValidableViewModel : ViewModel, IDataErrorInfo, INotifyDataErrorInfo
{
    private class PropertyValidator(Func<object> Getter, Func<object, bool> Validator, string? ErrorMessage)
    {
        public string? ErrorMessage { get; } = ErrorMessage;

        public bool IsValid => Validator(Getter());

        private bool IsPropertyValid(out string? Message)
        {
            var is_valid = IsValid;
            Message = is_valid ? null : ErrorMessage;
            return is_valid;
        }

        public string? GetErrorMessageIfInvalid() => IsPropertyValid(out var error) ? null : error;
    }

    private readonly Dictionary<string, List<PropertyValidator>> _Validators = [];

    protected ValidableViewModel(bool CheckDependencies = true)
        : base(CheckDependencies)
    {
        var type = GetType();
        foreach (var property in type.GetProperties().Where(p => p.CanRead))
        {
            var validator_attributes = property.GetCustomAttributes<ValidationAttribute>().ToArray();
            if (validator_attributes.Length == 0) continue;

            var getter = Expression
               .Property(this.ToExpression(), property.Name)
               .CreateLambda<Func<object>>()
               .Compile();

            static string GetAttributeName(ValidationAttribute attribute)
            {
                var          type           = attribute.GetType();
                var          type_name      = type.Name;
                const string attribute_name = "Attribute";
                return type_name.EndsWith(attribute_name, StringComparison.InvariantCulture)
                    ? type_name[..^attribute_name.Length]
                    : type_name;
            }

            _Validators.Add(property.Name, validator_attributes.Select(validator => new PropertyValidator(getter, validator.IsValid, validator.ErrorMessage ?? $"{property.Name} {GetAttributeName(validator)}")).ToList());
        }
    }

    #region IDataErrorInfo

    string IDataErrorInfo.this[string Property] =>
        _Validators.TryGetValue(Property, out var validators)
            ? validators
               .Where(validator => !validator.IsValid)
               .Select(validator => validator.ErrorMessage)
               .JoinStrings(Environment.NewLine)
            : string.Empty;

    string IDataErrorInfo.Error => _Validators.Keys
       .Select(property => ((IDataErrorInfo)this)[property])
       .JoinStrings(Environment.NewLine);

    #endregion

    #region INotifyDataErrorInfo

    IEnumerable INotifyDataErrorInfo.GetErrors(string? PropertyName) =>
        _Validators.TryGetValue(PropertyName ?? throw new ArgumentNullException(nameof(PropertyName)), out var validators)
            ? validators
               .Where(validator => !validator.IsValid)
               .Select(validator => validator.ErrorMessage)
            : [];

    bool INotifyDataErrorInfo.HasErrors => _Validators.Values.Any(pv => pv.Any(validator => !validator.IsValid));

    protected event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    event EventHandler<DataErrorsChangedEventArgs>? INotifyDataErrorInfo.ErrorsChanged
    {
        add => ErrorsChanged += value;
        remove => ErrorsChanged -= value;
    }

    protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e) => ErrorsChanged?.Invoke(this, e);

    #endregion

    protected override void OnPropertyChanged(string PropertyName = null!, bool UpdateCommandsState = false, bool ThreadSafe = true)
    {
        base.OnPropertyChanged(PropertyName, UpdateCommandsState, ThreadSafe);

        Validate(PropertyName);
    }

    private void Validate(string Property)
    {
        var event_handler = ErrorsChanged;
        if (event_handler is null) return;

        if (_Validators.TryGetValue(Property, out var validators) && validators.Any(v => !v.IsValid))
            OnErrorsChanged(new(Property));
    }
}