# GlobalHotKeys

Компоненты для регистрации глобальных горячих клавиш в WPF с привязкой к `ICommand`.

## Состав

- `GlobalHotKeysCollection` — коллекция горячих клавиш, подключаемая через attached‑свойство `UI.HotKeys`
- `GlobalHotKeyBinding` — описание одной горячей клавиши (клавиша, модификаторы, команда, параметр)

## Использование в XAML

```xml
<Window
    xmlns:local="clr-namespace:MathCore.WPF">
    <local:UI.HotKeys>
        <local:GlobalHotKeysCollection>
            <local:GlobalHotKeyBinding
                Key="F5"
                Command="{Binding RefreshCommand}" />
            <local:GlobalHotKeyBinding
                Key="S"
                Modifer="Control"
                Command="{Binding SaveCommand}" />
        </local:GlobalHotKeysCollection>
    </local:UI.HotKeys>
</Window>
```

## Использование в коде

```csharp
var hot_keys = UI.GetHotKeys(this);

hot_keys.Add(new GlobalHotKeyBinding
{
    Key = Keys.F5,
    Command = RefreshCommand
});

hot_keys.Add(new GlobalHotKeyBinding
{
    Key = Keys.S,
    Modifer = ModifierKeys.Control,
    Command = SaveCommand,
    CommandParameter = "autosave"
});
```

## Примечания

- Коллекция автоматически освобождает зарегистрированные хоткеи при выгрузке `Window`
- Горячие клавиши обрабатываются на уровне системных сообщений, поэтому работают вне фокуса элементов управления
