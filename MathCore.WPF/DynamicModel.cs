using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;

using MathCore.Annotations;

namespace MathCore.WPF;

[ContentProperty(nameof(Fields))]
public class DynamicModel : Freezable//, IList, IEnumerable
{
    //private readonly Dictionary<string, DynamicModelField> _Fields = new();

    public DynamicModelField this[string FieldName] => Fields[FieldName];

    public DynamicModelFieldsCollection Fields { get; } = new();

    private DynamicModelObject? _Model;

    public INotifyPropertyChanged Model => _Model ??= new(Fields);

    protected override Freezable CreateInstanceCore() => new DynamicModel();

    //#region IList

    //bool IList.IsFixedSize => false;
    //bool IList.IsReadOnly => false;

    //int ICollection.Count => _Fields.Count;

    //bool ICollection.IsSynchronized => false;

    //object ICollection.SyncRoot { get; } = new();

    //object? IList.this[int index]
    //{
    //    get => _Fields.Values.Select((v, i) => (v, i)).FirstOrDefault(v => v.i == index).v;
    //    set => throw new NotSupportedException();
    //}

    //int IList.Add(object? value)
    //{
    //    switch (value)
    //    {
    //        case null: throw new ArgumentNullException(nameof(value));
    //        default:
    //            throw new ArgumentException($"Значение типа {value.GetType()} не поддерживается. Требуется значение типа {typeof(DynamicModelField)}", nameof(value));
    //        case DynamicModelField { Name: { Length: > 0 } field_name } field:
    //            _Fields[field_name] = field;
    //            return _Fields.Count - 1;
    //        case DynamicModelField:
    //            throw new InvalidOperationException("Полю модели не задано имя");
    //    }
    //}

    //void IList.Clear() => _Fields.Clear();

    //bool IList.Contains(object? value) => value switch
    //{
    //    string name => _Fields.ContainsKey(name),
    //    DynamicModelField { Name: { Length: > 0 } name } => _Fields.ContainsKey(name),
    //    DynamicModelField => false,
    //    _ => throw new ArgumentException($"Значение типа {value.GetType()} не поддерживается. Требуется значение типа {typeof(DynamicModelField)}, либо {typeof(string)}", nameof(value))
    //};

    //int IList.IndexOf(object? value) => throw new NotSupportedException();

    //void IList.Insert(int index, object? value) => throw new NotSupportedException();

    //void IList.Remove(object? value)
    //{
    //    switch (value)
    //    {
    //        case null: throw new ArgumentNullException(nameof(value));
    //        default:
    //            throw new ArgumentException($"Значение типа {value.GetType()} не поддерживается. Требуется значение типа {typeof(DynamicModelField)}", nameof(value));
    //        case DynamicModelField { Name: { Length: > 0 } field_name }:
    //            _Fields.Remove(field_name);
    //            break;
    //        case DynamicModelField:
    //            throw new InvalidOperationException("Полю модели не задано имя");
    //        case string { Length: > 0 } name:
    //            _Fields.Remove(name);
    //            break;
    //        case string: throw new InvalidOperationException("Имя должно быть строкой ненулевой длины");
    //    }
    //}

    //void IList.RemoveAt(int index) => throw new NotImplementedException();
    //void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();

    //IEnumerator IEnumerable.GetEnumerator() => _Fields.Values.GetEnumerator();

    //#endregion
}

public class DynamicModelFieldsCollection : FreezableCollection<DynamicModelField>
{
    private readonly Dictionary<string, DynamicModelField> _Fields = new();

    public DynamicModelField this[string FieldName] => _Fields[FieldName];

    public DynamicModelFieldsCollection() => ((INotifyCollectionChanged)this).CollectionChanged += OnCollectionChanged;

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

    private readonly DynamicModelFieldsCollection _Fields;

    public DynamicModelObject(DynamicModelFieldsCollection Fields)
    {
        _Fields = Fields;
        ((INotifyCollectionChanged)Fields).CollectionChanged += OnFieldsCollectionChanged;
        AddEventHandler(Fields);
    }

    private void OnFieldsCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E)
    {
        if(Sender is not DynamicModelFieldsCollection fields) return;

        switch (E.Action)
        {
            case NotifyCollectionChangedAction.Add: AddEventHandler(E.NewItems.Cast<DynamicModelField>()); break;
            case NotifyCollectionChangedAction.Remove: RemoveEventHandler(E.NewItems.Cast<DynamicModelField>()); break;

            case NotifyCollectionChangedAction.Reset:
                ClearEventHandler();
                AddEventHandler(fields);
                break;

            default: throw new NotSupportedException($"Действие {E.Action} с коллекцией {typeof(DynamicModelFieldsCollection)} не поддерживается");
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
