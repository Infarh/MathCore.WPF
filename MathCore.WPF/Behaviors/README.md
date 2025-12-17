# MathCore.WPF Behaviors

Коллекция поведений (Behaviors) для WPF-приложений, расширяющих функциональность элементов управления без изменения их кода.

## 📋 Оглавление

- [Установка](#установка)
- [Поведения для перемещения и изменения размера](#поведения-для-перемещения-и-изменения-размера)
- [Поведения для окон](#поведения-для-окон)
- [Поведения для пользовательского ввода](#поведения-для-пользовательского-ввода)
- [Поведения для Drag & Drop](#поведения-для-drag--drop)
- [Поведения для привязки данных](#поведения-для-привязки-данных)
- [Поведения для списков и деревьев](#поведения-для-списков-и-деревьев)
- [Утилитарные поведения](#утилитарные-поведения)

---

## Установка

```xml
xmlns:b="http://schemas.microsoft.com/xaml/behaviors"
xmlns:behaviors="clr-namespace:MathCore.WPF.Behaviors;assembly=MathCore.WPF"
```

---

## Поведения для перемещения и изменения размера

### DragBehavior

Универсальное поведение для перетаскивания элементов внутри различных контейнеров (Canvas, Panel, ContentControl).

**Основные свойства:**
- `Enabled` - включение/выключение перетаскивания
- `Xmin`, `Xmax`, `Ymin`, `Ymax` - ограничения области перемещения
- `AllowX`, `AllowY` - разрешение перемещения по осям
- `CurrentX`, `CurrentY` - текущие координаты (readonly)
- `dx`, `dy` - смещение от начальной точки (readonly)
- `Radius`, `Angle` - полярные координаты смещения (readonly)

**Пример использования:**

```xml
<Border Background="LightBlue" Width="100" Height="100">
    <b:Interaction.Behaviors>
        <behaviors:DragBehavior 
            Enabled="True"
            Xmin="0" Xmax="400"
            Ymin="0" Ymax="300"
            AllowX="True" AllowY="True"/>
    </b:Interaction.Behaviors>
</Border>
```

**Пример с ограничением только по горизонтали:**

```xml
<Rectangle Fill="Red" Width="50" Height="50">
    <b:Interaction.Behaviors>
        <behaviors:DragBehavior 
            AllowX="True" 
            AllowY="False"
            Xmin="0" Xmax="500"/>
    </b:Interaction.Behaviors>
</Rectangle>
```

---

### DragInCanvasBehavior

Специализированное поведение для перетаскивания элементов внутри Canvas с расширенными возможностями.

**Основные свойства:**
- `Enabled` - включение/выключение перетаскивания
- `Xmin`, `Xmax`, `Ymin`, `Ymax` - ограничения области перемещения
- `AllowX`, `AllowY` - разрешение перемещения по осям
- `CurrentX`, `CurrentY` - текущие координаты с двусторонней привязкой

**Пример использования:**

```xml
<Canvas>
    <Ellipse Canvas.Left="50" Canvas.Top="50" 
             Fill="Green" Width="60" Height="60">
        <b:Interaction.Behaviors>
            <behaviors:DragInCanvasBehavior 
                Enabled="True"
                Xmin="0" Xmax="{Binding ActualWidth, RelativeSource={RelativeSource AncestorType=Canvas}}"
                Ymin="0" Ymax="{Binding ActualHeight, RelativeSource={RelativeSource AncestorType=Canvas}}"
                CurrentX="{Binding PositionX, Mode=TwoWay}"
                CurrentY="{Binding PositionY, Mode=TwoWay}"/>
        </b:Interaction.Behaviors>
    </Ellipse>
</Canvas>
```

---

### TranslateMoveBehavior

Поведение для перемещения элемента с использованием `TranslateTransform` без изменения его логической позиции.

**Пример использования:**

```xml
<Image Source="logo.png">
    <b:Interaction.Behaviors>
        <behaviors:TranslateMoveBehavior/>
    </b:Interaction.Behaviors>
</Image>
```

---

### DragWindow

Поведение для перетаскивания окна за любой элемент внутри него.

**Пример использования:**

```xml
<Grid Background="DarkGray">
    <b:Interaction.Behaviors>
        <behaviors:DragWindow/>
    </b:Interaction.Behaviors>
    
    <TextBlock Text="Перетащите окно за эту область" 
               HorizontalAlignment="Center" 
               VerticalAlignment="Center"/>
</Grid>
```

---

### Resize

Поведение для визуализации областей изменения размера элемента управления (отображение курсоров).

**Основные свойства:**
- `AreaSize` - размер области захвата в пикселях (по умолчанию 3)
- `TopResizing`, `BottomResizing`, `LeftResizing`, `RightResizing` - включение изменения размера с соответствующих сторон

**Пример использования:**

```xml
<Border BorderBrush="Black" BorderThickness="1">
    <b:Interaction.Behaviors>
        <behaviors:Resize 
            AreaSize="5"
            TopResizing="True"
            BottomResizing="True"
            LeftResizing="True"
            RightResizing="True"/>
    </b:Interaction.Behaviors>
</Border>
```

---

### ResizeBehavior

Расширенное поведение для изменения размера окна.

**Пример использования:**

```xml
<Window>
    <b:Interaction.Behaviors>
        <behaviors:ResizeBehavior/>
    </b:Interaction.Behaviors>
</Window>
```

---

### ResizeWindowPanel

Поведение для создания панели изменения размера окна.

**Пример использования:**

```xml
<Grid>
    <b:Interaction.Behaviors>
        <behaviors:ResizeWindowPanel/>
    </b:Interaction.Behaviors>
</Grid>
```

---

## Поведения для окон

### WindowBehavior

Абстрактный базовый класс для поведений, работающих с окнами. Предоставляет доступ к родительскому окну через свойство `AssociatedWindow`.

---

### CloseBehavior

Поведение для окна с поддержкой анимации закрытия и установки `DialogResult`.

**Основные свойства:**
- `CloseWithDialogResult` - триггер закрытия окна с установкой результата диалога
- `Storyboard` - анимация, которая будет воспроизведена перед закрытием окна

**Пример использования:**

```xml
<Window>
    <Window.Resources>
        <Storyboard x:Key="FadeOutStoryboard">
            <DoubleAnimation Storyboard.TargetProperty="Opacity"
                           From="1" To="0" Duration="0:0:0.3"/>
        </Storyboard>
    </Window.Resources>
    
    <b:Interaction.Behaviors>
        <behaviors:CloseBehavior 
            CloseWithDialogResult="{Binding ShouldClose}"
            Storyboard="{StaticResource FadeOutStoryboard}"/>
    </b:Interaction.Behaviors>
</Window>
```

---

### CloseButtonBehavior

Поведение для кнопки, закрывающей родительское окно.

**Пример использования:**

```xml
<Button Content="Закрыть">
    <b:Interaction.Behaviors>
        <behaviors:CloseButtonBehavior/>
    </b:Interaction.Behaviors>
</Button>
```

---

### WindowSystemIconBehavior

Поведение для отображения системной иконки окна.

**Пример использования:**

```xml
<Image>
    <b:Interaction.Behaviors>
        <behaviors:WindowSystemIconBehavior/>
    </b:Interaction.Behaviors>
</Image>
```

---

### WindowTitleBarBehavior

Поведение для создания пользовательской области заголовка окна.

**Пример использования:**

```xml
<Grid Background="Navy" Height="30">
    <b:Interaction.Behaviors>
        <behaviors:WindowTitleBarBehavior/>
    </b:Interaction.Behaviors>
    
    <TextBlock Text="{Binding Title, RelativeSource={RelativeSource AncestorType=Window}}"
               Foreground="White"
               VerticalAlignment="Center"
               Margin="10,0"/>
</Grid>
```

---

### WindowMaximizationLimitattor

Поведение для ограничения максимизации окна с учётом рабочей области экрана.

**Пример использования:**

```xml
<Window>
    <b:Interaction.Behaviors>
        <behaviors:WindowMaximizationLimitattor/>
    </b:Interaction.Behaviors>
</Window>
```

---

## Поведения для пользовательского ввода

### UserInputBehavior

Поведение для обработки событий мыши и клавиатуры через команды.

**Основные свойства:**
- `LeftMouseDownCommand` - команда при нажатии левой кнопки мыши
- `LeftMouseUpCommand` - команда при отпускании левой кнопки мыши
- `MouseWheelCommand` - команда при прокрутке колеса мыши
- `KeyDownCommand` - команда при нажатии клавиши
- `KeyUpCommand` - команда при отпускании клавиши
- `Position` - текущая позиция мыши (readonly)

**Пример использования:**

```xml
<Canvas Background="White">
    <b:Interaction.Behaviors>
        <behaviors:UserInputBehavior 
            LeftMouseDownCommand="{Binding MouseDownCommand}"
            MouseWheelCommand="{Binding ZoomCommand}"
            KeyDownCommand="{Binding KeyPressCommand}"
            Position="{Binding MousePosition, Mode=OneWayToSource}"/>
    </b:Interaction.Behaviors>
</Canvas>
```

---

### MouseControlBehavior

Поведение для отслеживания состояния мыши и её положения относительно элемента.

**Основные свойства:**
- `MousePosition` - абсолютная позиция мыши
- `MousePositionRelative` - относительная позиция (0..1)
- `ElementSize` - размер элемента
- `IsLeftMouseDown` - состояние левой кнопки мыши
- `LeftMouseClick` - команда при клике

**Пример использования:**

```xml
<Border Background="LightGray" Width="300" Height="200">
    <b:Interaction.Behaviors>
        <behaviors:MouseControlBehavior 
            MousePosition="{Binding AbsoluteMousePos, Mode=OneWayToSource}"
            MousePositionRelative="{Binding RelativeMousePos, Mode=OneWayToSource}"
            IsLeftMouseDown="{Binding IsMousePressed, Mode=OneWayToSource}"
            LeftMouseClick="{Binding ClickCommand}"/>
    </b:Interaction.Behaviors>
    
    <TextBlock>
        <TextBlock.Text>
            <MultiBinding StringFormat="Mouse: {0:F0}, {1:F0}">
                <Binding Path="AbsoluteMousePos.X"/>
                <Binding Path="AbsoluteMousePos.Y"/>
            </MultiBinding>
        </TextBlock.Text>
    </TextBlock>
</Border>
```

---

## Поведения для Drag & Drop

### DropFile

Поведение для получения файлов через Drag-and-Drop.

**Основные свойства:**
- `DropFileCommand` - команда, вызываемая при получении файла (параметр - `FileInfo`)

**Пример использования:**

```xml
<Border BorderBrush="DashedGray" BorderThickness="2" 
        Background="WhiteSmoke" 
        Width="400" Height="300">
    <b:Interaction.Behaviors>
        <behaviors:DropFile DropFileCommand="{Binding HandleDroppedFileCommand}"/>
    </b:Interaction.Behaviors>
    
    <TextBlock Text="Перетащите файл сюда" 
               HorizontalAlignment="Center" 
               VerticalAlignment="Center"
               FontSize="16"/>
</Border>
```

**Пример команды в ViewModel:**

```csharp
public ICommand HandleDroppedFileCommand => new LambdaCommand(
    file =>
    {
        if (file is FileInfo fileInfo)
        {
            FilePath = fileInfo.FullName;
            // Дополнительная обработка файла
        }
    });
```

---

### DropData

Поведение для получения произвольных данных через Drag-and-Drop.

**Основные свойства:**
- `DropDataCommand` - команда, вызываемая при получении данных
- `DataFormat` - формат принимаемых данных

**Пример использования:**

```xml
<ListBox AllowDrop="True">
    <b:Interaction.Behaviors>
        <behaviors:DropData 
            DropDataCommand="{Binding HandleDropCommand}"
            DataFormat="{x:Static DataFormats.Text}"/>
    </b:Interaction.Behaviors>
</ListBox>
```

---

## Поведения для привязки данных

### ActualSizeBinding

Поведение для двухсторонней привязки фактических размеров элемента.

**Основные свойства:**
- `ActualWidth` - ширина элемента (двусторонняя привязка)
- `ActualHeight` - высота элемента (двусторонняя привязка)

**Пример использования:**

```xml
<Border Background="Blue">
    <b:Interaction.Behaviors>
        <behaviors:ActualSizeBinding 
            ActualWidth="{Binding ElementWidth, Mode=TwoWay}"
            ActualHeight="{Binding ElementHeight, Mode=TwoWay}"/>
    </b:Interaction.Behaviors>
</Border>

<!-- Отображение размеров -->
<TextBlock Text="{Binding ElementWidth, StringFormat='Ширина: {0:F0} px'}"/>
<TextBlock Text="{Binding ElementHeight, StringFormat='Высота: {0:F0} px'}"/>
```

---

### PasswordBoxBinder

Поведение для привязки пароля из `PasswordBox` к ViewModel (использовать с осторожностью из соображений безопасности).

**Основные свойства:**
- `Password` - пароль для привязки

**Пример использования:**

```xml
<PasswordBox>
    <b:Interaction.Behaviors>
        <behaviors:PasswordBoxBinder Password="{Binding UserPassword, Mode=TwoWay}"/>
    </b:Interaction.Behaviors>
</PasswordBox>
```

⚠️ **Предупреждение:** Хранение пароля в виде обычной строки снижает безопасность. Используйте `SecureString` где возможно.

---

## Поведения для списков и деревьев

### ListBoxItemsSelectionBinder

Поведение для двухсторонней привязки выделенных элементов ListBox.

**Основные свойства:**
- `SelectedItems` - коллекция выбранных элементов

**Пример использования:**

```xml
<ListBox SelectionMode="Multiple" ItemsSource="{Binding Items}">
    <b:Interaction.Behaviors>
        <behaviors:ListBoxItemsSelectionBinder 
            SelectedItems="{Binding SelectedItems}"/>
    </b:Interaction.Behaviors>
</ListBox>
```

**ViewModel:**

```csharp
public ObservableCollection<Item> SelectedItems { get; } = new();
```

---

### ListBoxScrollOnNewItem

Поведение для автоматической прокрутки ListBox при добавлении новых элементов.

**Пример использования:**

```xml
<ListBox ItemsSource="{Binding LogMessages}">
    <b:Interaction.Behaviors>
        <behaviors:ListBoxScrollOnNewItem/>
    </b:Interaction.Behaviors>
</ListBox>
```

---

### TreeViewBindableSelectedItem

Поведение для привязки выбранного элемента TreeView (обходит ограничение WPF).

**Основные свойства:**
- `SelectedItem` - выбранный элемент

**Пример использования:**

```xml
<TreeView ItemsSource="{Binding TreeItems}">
    <b:Interaction.Behaviors>
        <behaviors:TreeViewBindableSelectedItem 
            SelectedItem="{Binding SelectedTreeItem, Mode=TwoWay}"/>
    </b:Interaction.Behaviors>
</TreeView>
```

---

### TreeViewBindableSelectedDirectoryViewModelItem

Специализированное поведение для TreeView с элементами типа `DirectoryViewModel`.

**Пример использования:**

```xml
<TreeView ItemsSource="{Binding DirectoryTree}">
    <b:Interaction.Behaviors>
        <behaviors:TreeViewBindableSelectedDirectoryViewModelItem 
            SelectedItem="{Binding SelectedDirectory, Mode=TwoWay}"/>
    </b:Interaction.Behaviors>
</TreeView>
```

---

## Утилитарные поведения

### Focused

Поведение для автоматической установки фокуса на элемент при его инициализации.

**Пример использования:**

```xml
<TextBox>
    <b:Interaction.Behaviors>
        <behaviors:Focused/>
    </b:Interaction.Behaviors>
</TextBox>
```

---

### TextBoxSelectAllAtGotFocus

Поведение для автоматического выделения всего текста в TextBox при получении фокуса.

**Пример использования:**

```xml
<TextBox Text="Редактируемый текст">
    <b:Interaction.Behaviors>
        <behaviors:TextBoxSelectAllAtGotFocus/>
    </b:Interaction.Behaviors>
</TextBox>
```

---

### TextBoxEndCaretAtGotFocus

Поведение для установки каретки в конец текста TextBox при получении фокуса.

**Пример использования:**

```xml
<TextBox Text="Текст для редактирования">
    <b:Interaction.Behaviors>
        <behaviors:TextBoxEndCaretAtGotFocus/>
    </b:Interaction.Behaviors>
</TextBox>
```

---

## 📚 Дополнительные материалы

### Установка пакета Microsoft.Xaml.Behaviors

Для использования поведений необходим пакет:

```bash
dotnet add package Microsoft.Xaml.Behaviors.Wpf
```

### Общие рекомендации

1. **Производительность:** Используйте поведения разумно - каждое поведение добавляет обработчики событий
2. **Память:** Поведения автоматически отписываются от событий при отсоединении
3. **MVVM:** Поведения - отличный способ избежать code-behind и сохранить чистоту MVVM
4. **Тестирование:** Поведения можно тестировать независимо от UI

### Создание собственного поведения

```csharp
public class CustomBehavior : Behavior<FrameworkElement>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        // Подписка на события
        AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        // Отписка от событий
        AssociatedObject.Loaded -= OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Логика поведения
    }
}
```

---

## 🐛 Известные проблемы и ограничения

1. **Resize.cs** - В текущей версии только отображает курсоры, фактическое изменение размера не реализовано
2. **PasswordBoxBinder** - Снижает безопасность при использовании обычных строк
3. **DragBehavior и DragInCanvasBehavior** - Имеют дублирование кода, планируется унификация

Подробнее см. [BehaviorsAnalysisReport.md](../../BehaviorsAnalysisReport.md) и [BehaviorsImprovementRecommendations.md](../../BehaviorsImprovementRecommendations.md)

---

## 📝 Лицензия

Этот проект является частью библиотеки MathCore.WPF.

---

## 🤝 Вклад в проект

Contributions are welcome! Если вы нашли баг или хотите предложить улучшение:

1. Создайте Issue с описанием проблемы
2. Fork репозиторий
3. Создайте ветку для ваших изменений
4. Отправьте Pull Request

---

## 📧 Контакты

- GitHub: [MathCore.WPF](https://github.com/Infarh/MathCore.WPF)
- Issues: [GitHub Issues](https://github.com/Infarh/MathCore.WPF/issues)
