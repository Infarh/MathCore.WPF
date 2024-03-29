﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace MathCore.WPF.Templates.Selectors;

[ContentProperty("Templates")]
public class CustomDataTemplateSelector : DataTemplateSelector
{
    private readonly Dictionary<string, DataTemplate> _Items;
    public IDictionary<string, DataTemplate> Templates => _Items;

    public Func<object, string> KeySelector { get; set; }

    public CustomDataTemplateSelector()
    {
        _Items      = [];
        KeySelector = o => o.GetType().Name;
    }

    public CustomDataTemplateSelector(Dictionary<string, DataTemplate> Items, Func<object, string> KeySelector)
    {
        _Items           = Items;
        this.KeySelector = KeySelector;
    }

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container) => 
        item != null && _Items.TryGetValue(KeySelector(item), out var t) ? t : null;
}