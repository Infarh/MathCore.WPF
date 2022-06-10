using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.Annotations;

namespace MathCore.WPF;

public class DynamicModelContent : Decorator
{
    #region Attached property Model : DynamicModel - Модель

    /// <summary>Модель</summary>
    [AttachedPropertyBrowsableForType(typeof(DynamicModelContent))]
    public static void SetModel(DependencyObject d, DynamicModel value) => d.SetValue(ModelProperty, value);

    /// <summary>Модель</summary>
    public static DynamicModel GetModel(DependencyObject d)
    {
        if(d.GetValue(ModelProperty) is DynamicModel model) return model;
        model = new(); 
        SetModel(d, model);
        return model;
    }

    /// <summary>Модель</summary>
    public static readonly DependencyProperty ModelProperty =
        DependencyProperty.RegisterAttached(
            "ModelShadow",
            typeof(DynamicModel),
            typeof(DynamicModelContent),
            new PropertyMetadata(default(DynamicModel)));

    #endregion

    protected override void OnVisualChildrenChanged(DependencyObject? VisualAdded, DependencyObject? VisualRemoved)
    {
        if (VisualRemoved is not null)
            BindingOperations.ClearBinding(VisualRemoved, DataContextProperty);

        if (VisualAdded is { })
        {
            /*"(DynamicModelContent.ModelShadow).Model"*/
            var pr = ModelProperty;
            var n = pr.Name;
            BindingOperations.SetBinding(
                VisualAdded, 
                DataContextProperty, new Binding($"({nameof(DynamicModelContent)}.{ModelProperty.Name}).{nameof(DynamicModel.Model)}")
                {
                    Source = this,
                    //Path = new PropertyPath("({0}.{1}).{2}",
                    //    nameof(DynamicModelContent), 
                    //    ModelProperty.Name, 
                    //    nameof(DynamicModel.Model))
                });
        }

        base.OnVisualChildrenChanged(VisualAdded, VisualRemoved);
    }
}

public class DynamicModel : FreezableCollection<DynamicModelField>
{
    private readonly Dictionary<string, DynamicModelField> _Fields = new();

    public DynamicModelField this[string FieldName] => _Fields[FieldName];

    public DynamicModel() => ((INotifyCollectionChanged)this).CollectionChanged += OnCollectionChanged;

    public bool ContainsField(string FieldName) => _Fields.ContainsKey(FieldName);

    public DynamicModelField? GetField(string FieldName) => _Fields.TryGetValue(FieldName, out var field) ? field : null;

    public bool TryGetField(string FieldName, out DynamicModelField field) => _Fields.TryGetValue(FieldName, out field);

    private void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E)
    {
        switch (E.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (DynamicModelField field in E.NewItems)
                    _Fields[field.Name] = field;
                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (DynamicModelField field in E.OldItems)
                    _Fields.Remove(field.Name);
                break;

            case NotifyCollectionChangedAction.Reset:
                _Fields.Clear();
                foreach (var field in this)
                    _Fields[field.Name] = field;
                break;

            default: throw new NotSupportedException($"Действие {E.Action} с коллекцией полей динамической модели не поддерживается");
        }
    }

    private DynamicModelObject? _Model;

    public INotifyPropertyChanged Model => _Model ??= new(this);
}

[ContentProperty(nameof(Value))]
[Bindable(true, BindingDirection.TwoWay)]
[DefaultBindingProperty(nameof(Value))]
[DefaultProperty(nameof(Value))]
public class DynamicModelField : Freezable, INotifyPropertyChanged
{
    protected override Freezable CreateInstanceCore() => new DynamicModelField();

    public string Name { get; set; }

    #region Value : object - Значение поля

    /// <summary>Значение поля</summary>
    //[Category("")]
    [Description("Значение поля")]
    [Bindable(true, BindingDirection.TwoWay)]
    public object Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

    /// <summary>Значение поля</summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(object),
            typeof(DynamicModelField),
            new PropertyMetadata(default, (d, _) => ((DynamicModelField)d).OnPropertyChanged(nameof(Value))));

    #endregion

    public override string ToString() => $"Field[{Name}]={Value}";
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? PropertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
}

internal sealed class DynamicModelObject : DynamicObject, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? PropertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

    private readonly DynamicModel _Fields;

    public DynamicModelObject(DynamicModel Fields)
    {
        _Fields = Fields;
        ((INotifyCollectionChanged)Fields).CollectionChanged += OnFieldsCollectionChanged;
        AddEventHandler(Fields);
    }

    private void OnFieldsCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E)
    {
        if (Sender is not DynamicModel fields) return;

        switch (E.Action)
        {
            case NotifyCollectionChangedAction.Add: AddEventHandler(E.NewItems.Cast<DynamicModelField>()); break;
            case NotifyCollectionChangedAction.Remove: RemoveEventHandler(E.NewItems.Cast<DynamicModelField>()); break;

            case NotifyCollectionChangedAction.Reset:
                ClearEventHandler();
                AddEventHandler(fields);
                break;

            default: throw new NotSupportedException($"Действие {E.Action} с коллекцией {typeof(DynamicModel)} не поддерживается");
        }
    }

    private void ClearEventHandler()
    {
        foreach (var field in _HandledFields)
            field.PropertyChanged -= OnFieldPropertyChanged;
        _HandledFields.Clear();
    }

    private readonly List<DynamicModelField> _HandledFields = new();


    private void AddEventHandler(IEnumerable<DynamicModelField> fields)
    {
        foreach (var field in fields)
        {
            field.PropertyChanged += OnFieldPropertyChanged;
            _HandledFields.Add(field);
        }
    }

    private void RemoveEventHandler(IEnumerable<DynamicModelField> fields)
    {
        foreach (var field in fields)
        {
            field.PropertyChanged -= OnFieldPropertyChanged;
            _HandledFields.Remove(field);
        }
    }

    private void OnFieldPropertyChanged(object? Sender, PropertyChangedEventArgs E)
    {
        if (E.PropertyName == nameof(DynamicModelField.Value) && Sender is DynamicModelField field)
            OnPropertyChanged(field.Name);
    }

    public override IEnumerable<string> GetDynamicMemberNames() => _Fields.Select(f => f.Name);

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        var field_name = binder.Name;
        if (!_Fields.TryGetField(field_name, out var field))
        {
            result = null;
            return false;
        }

        result = field.Value;
        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        var field_name = binder.Name;
        if (!_Fields.TryGetField(field_name, out var field))
            return false;
        field.Value = value;
        return true;
    }
}
