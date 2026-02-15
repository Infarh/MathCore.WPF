# MathCore.WPF.Shaders

Модуль содержит набор пиксельных шейдеров для применения различных визуальных эффектов к элементам WPF. Все эффекты наследуются от `System.Windows.Media.Effects.ShaderEffect` и используют предскомпилированные пиксельные шейдеры (HLSL), встроенные в сборку.

## Содержание

- [Основные концепции](#основные-концепции)
- [Blur](#blur) — стандартное размытие
- [DirectionalBlur](#directionalblur) — направленное размытие
- [ZoomBlur](#zoomblur) — размытие из центра
- [GrayScale](#grayscale) — оттенки серого
- [BlackAndWhite](#blackandwhite) — чёрно-белое изображение
- [Invert](#invert) — инвертирование цветов
- [Sepia](#sepia) — эффект сепии
- [Opacity](#opacity) — контроль прозрачности
- [ColorAlphaKey](#coloralphakey) — установка прозрачного цвета
- [Рекомендации по использованию](#рекомендации-по-использованию)
- [Примеры](#примеры)

---

## Основные концепции

### Что такое ShaderEffect?

`ShaderEffect` — это базовый класс для применения пиксельных шейдеров к элементам WPF. Шейдеры компилируются из HLSL кода (High-Level Shading Language) в промежуточное представление (*.ps файлы) и встраиваются в сборку как ресурсы.

### Как работают эффекты?

1. Все эффекты наследуются от `ShaderEffect`
2. Свойство `Input` получает исходное изображение (обычно устанавливается автоматически)
3. Дополнительные свойства контролируют параметры эффекта (например, `Factor`, `BlurAmount`)
4. Результат применяется к визуальному элементу в реальном времени

### Свойство Input

Все эффекты имеют свойство `Input` типа `Brush`, которое:
- Автоматически устанавливается системой WPF
- Представляет исходное изображение элемента
- Помечено атрибутом `[Browsable(false)]`, так как не предназначено для ручного установки

### Производительность

- Эффекты выполняются на графическом процессоре (GPU)
- Лучше всего работают на современных видеокартах
- Может быть снижение производительности на очень старых GPU
- Рекомендуется избегать применения сложных эффектов к большому количеству элементов одновременно

---

## Blur

Эффект стандартного размытия, размывающего изображение во всех направлениях одинаково.

### Описание

`Blur` — это эффект, который размывает изображение, делая его мягче и менее четким. Интенсивность размытия контролируется свойством `Factor`.

### Основные свойства

| Свойство | Тип | Значение по умолчанию | Описание |
|----------|-----|----------------------|---------|
| **Input** | `Brush` | автоматически | Исходное изображение (устанавливается системой WPF) |
| **Factor** | `float` | `0.5f` | Интенсивность размытия (0.0 = без размытия, 1.0 = максимальное размытие) |

### Примеры использования

#### В XAML

```xaml
<Window
    xmlns:effects="clr-namespace:MathCore.WPF.Shaders;assembly=MathCore.WPF">
    <Grid>
        <Image Source="image.jpg">
            <Image.Effect>
                <effects:Blur Factor="0.5" />
            </Image.Effect>
        </Image>
    </Grid>
</Window>
```

#### Анимация размытия

```xaml
<Grid>
    <Image Source="image.jpg">
        <Image.Effect>
            <effects:Blur x:Name="BlurEffect" Factor="0" />
        </Image.Effect>
        <Grid.Triggers>
            <EventTrigger RoutedEvent="Grid.MouseEnter">
                <BeginStoryboard>
                    <Storyboard>
                        <DoubleAnimation
                            Storyboard.TargetName="BlurEffect"
                            Storyboard.TargetProperty="Factor"
                            To="1.0"
                            Duration="0:0:0.5" />
                    </Storyboard>
                </BeginStoryboard>
            </EventTrigger>
        </Grid.Triggers>
    </Image>
</Grid>
```

#### В коде

```csharp
var image = new Image { Source = new BitmapImage(new Uri("image.jpg", UriKind.Relative)) };
image.Effect = new Blur { Factor = 0.7f };
canvas.Children.Add(image);

// Анимация
var animation = new DoubleAnimation
{
    From = 0,
    To = 1,
    Duration = TimeSpan.FromSeconds(1),
    AutoReverse = true
};
image.Effect.BeginAnimation(Blur.FactorProperty, animation);
```

---

## DirectionalBlur

Размытие в определённом направлении. Размывает изображение вдоль прямой линии.

### Описание

`DirectionalBlur` — это эффект, который размывает изображение в указанном направлении. Полезен для имитации движения или создания специальных визуальных эффектов.

### Основные свойства

| Свойство | Тип | Значение по умолчанию | Описание |
|----------|-----|----------------------|---------|
| **Input** | `Brush` | автоматически | Исходное изображение (устанавливается системой WPF) |
| **Angle** | `double` | `0.0` | Направление размытия в градусах (0° вправо, 90° вниз) |
| **BlurAmount** | `double` | `0.1` | Интенсивность размытия (0.0 = без размытия, >0 = размутие) |

### Примеры использования

#### Горизонтальное размытие (имитация движения)

```xaml
<Grid>
    <Image Source="image.jpg">
        <Image.Effect>
            <effects:DirectionalBlur Angle="0" BlurAmount="0.2" />
        </Image.Effect>
    </Image>
</Grid>
```

#### Вертикальное размытие

```xaml
<Image Source="image.jpg">
    <Image.Effect>
        <effects:DirectionalBlur Angle="90" BlurAmount="0.15" />
    </Image.Effect>
</Image>
```

#### Диагональное размытие

```xaml
<Image Source="image.jpg">
    <Image.Effect>
        <effects:DirectionalBlur Angle="45" BlurAmount="0.2" />
    </Image.Effect>
</Image>
```

#### С вращением направления в коде

```csharp
var image = new Image { Source = new BitmapImage(new Uri("image.jpg", UriKind.Relative)) };
var blur = new DirectionalBlur { BlurAmount = 0.2 };
image.Effect = blur;

// Анимация вращения направления
var animation = new DoubleAnimation
{
    From = 0,
    To = 360,
    Duration = TimeSpan.FromSeconds(5),
    RepeatBehavior = RepeatBehavior.Forever
};
blur.BeginAnimation(DirectionalBlur.AngleProperty, animation);

canvas.Children.Add(image);
```

---

## ZoomBlur

Размытие, исходящее из центральной точки, создающее эффект приближения или отдаления камеры.

### Описание

`ZoomBlur` — это эффект, который создаёт иллюзию движения из указанной центральной точки во все стороны. Полезен для создания эффектов прыжков, взрывов или быстрого приближения.

### Основные свойства

| Свойство | Тип | Значение по умолчанию | Описание |
|----------|-----|----------------------|---------|
| **Input** | `Brush` | автоматически | Исходное изображение (устанавливается системой WPF) |
| **Center** | `Point` | `(0.5, 0.5)` | Центр размытия в нормализованных координатах (0.0–1.0). (0.5, 0.5) = центр элемента |
| **BlurAmount** | `double` | `0.1` | Интенсивность размытия (0.0 = без размытия, >0 = размытие) |

### Примеры использования

#### По центру элемента

```xaml
<Image Source="image.jpg" Width="300" Height="300">
    <Image.Effect>
        <effects:ZoomBlur Center="0.5,0.5" BlurAmount="0.15" />
    </Image.Effect>
</Image>
```

#### По левому верхнему углу

```xaml
<Image Source="image.jpg">
    <Image.Effect>
        <effects:ZoomBlur Center="0,0" BlurAmount="0.2" />
    </Image.Effect>
</Image>
```

#### Из верхней части изображения

```xaml
<Image Source="image.jpg">
    <Image.Effect>
        <effects:ZoomBlur Center="0.5,0" BlurAmount="0.25" />
    </Image.Effect>
</Image>
```

#### С анимацией центра в коде

```csharp
var image = new Image { Source = new BitmapImage(new Uri("image.jpg", UriKind.Relative)) };
var zoomBlur = new ZoomBlur { BlurAmount = 0.2 };
image.Effect = zoomBlur;

// Анимация движения центра
var centerAnimation = new PointAnimation
{
    From = new Point(0.25, 0.25),
    To = new Point(0.75, 0.75),
    Duration = TimeSpan.FromSeconds(2),
    AutoReverse = true,
    RepeatBehavior = RepeatBehavior.Forever
};
zoomBlur.BeginAnimation(ZoomBlur.CenterProperty, centerAnimation);

canvas.Children.Add(image);
```

---

## GrayScale

Преобразование изображения в оттенки серого (ч/б).

### Описание

`GrayScale` — это эффект, который преобразует цветное изображение в оттенки серого. Свойство `Factor` позволяет плавно переходить от цветного изображения к чёрно-белому.

### Основные свойства

| Свойство | Тип | Значение по умолчанию | Описание |
|----------|-----|----------------------|---------|
| **Input** | `Brush` | автоматически | Исходное изображение (устанавливается системой WPF) |
| **Factor** | `float` | `1.0f` | Степень десатурации (0.0 = полностью цветной, 1.0 = полностью ч/б) |

### Примеры использования

#### Полностью чёрно-белое

```xaml
<Image Source="image.jpg">
    <Image.Effect>
        <effects:GrayScale Factor="1.0" />
    </Image.Effect>
</Image>
```

#### Частичная десатурация (50%)

```xaml
<Image Source="image.jpg">
    <Image.Effect>
        <effects:GrayScale Factor="0.5" />
    </Image.Effect>
</Image>
```

#### Переход в ч/б при наведении мыши

```xaml
<Grid>
    <Image Source="image.jpg" x:Name="ColorImage">
        <Image.Effect>
            <effects:GrayScale x:Name="GrayScaleEffect" Factor="0" />
        </Image.Effect>
        <Grid.Triggers>
            <EventTrigger RoutedEvent="Grid.MouseEnter">
                <BeginStoryboard>
                    <Storyboard>
                        <DoubleAnimation
                            Storyboard.TargetName="GrayScaleEffect"
                            Storyboard.TargetProperty="Factor"
                            To="1.0"
                            Duration="0:0:0.3" />
                    </Storyboard>
                </BeginStoryboard>
            </EventTrigger>
            <EventTrigger RoutedEvent="Grid.MouseLeave">
                <BeginStoryboard>
                    <Storyboard>
                        <DoubleAnimation
                            Storyboard.TargetName="GrayScaleEffect"
                            Storyboard.TargetProperty="Factor"
                            To="0"
                            Duration="0:0:0.3" />
                    </Storyboard>
                </BeginStoryboard>
            </EventTrigger>
        </Grid.Triggers>
    </Image>
</Grid>
```

#### В коде

```csharp
var image = new Image { Source = new BitmapImage(new Uri("image.jpg", UriKind.Relative)) };
image.Effect = new GrayScale { Factor = 0.75f };
canvas.Children.Add(image);
```

---

## BlackAndWhite

Преобразование в чёрное и белое (без оттенков серого).

### Описание

`BlackAndWhite` — это эффект, который преобразует изображение в чистое чёрно-белое изображение без промежуточных оттенков серого. Свойство `Factor` контролирует степень применения эффекта.

### Основные свойства

| Свойство | Тип | Значение по умолчанию | Описание |
|----------|-----|----------------------|---------|
| **Input** | `Brush` | автоматически | Исходное изображение (устанавливается системой WPF) |
| **Factor** | `float` | `1.0f` | Степень применения эффекта (0.0 = оригинальное, 1.0 = полный ч/б) |

### Примеры использования

#### Полностью чёрно-белое

```xaml
<Image Source="image.jpg">
    <Image.Effect>
        <effects:BlackAndWhite Factor="1.0" />
    </Image.Effect>
</Image>
```

#### Частичное применение (50%)

```xaml
<Image Source="image.jpg">
    <Image.Effect>
        <effects:BlackAndWhite Factor="0.5" />
    </Image.Effect>
</Image>
```

---

## Invert

Инвертирование всех цветов изображения.

### Описание

`Invert` — это эффект, который инвертирует все цвета изображения (чёрное становится белым, синее становится жёлтым и т.д.). Не имеет дополнительных параметров, так как эффект либо применяется, либо нет.

### Основные свойства

| Свойство | Тип | Значение по умолчанию | Описание |
|----------|-----|----------------------|---------|
| **Input** | `Brush` | автоматически | Исходное изображение (устанавливается системой WPF) |

### Примеры использования

#### Инвертирование цветов

```xaml
<Image Source="image.jpg">
    <Image.Effect>
        <effects:Invert />
    </Image.Effect>
</Image>
```

#### С фильтром Invert для кнопки (негатив при нажатии)

```xaml
<Button Width="100" Height="40" Background="Blue">
    <Button.Style>
        <Style TargetType="Button">
            <Style.Triggers>
                <Trigger Property="IsPressed" Value="True">
                    <Setter Property="Effect" Value="{x:Static local:InvertEffectInstance}" />
                </Trigger>
            </Style.Triggers>
        </Style>
    </Button.Style>
    Инвертировать
</Button>
```

#### В коде

```csharp
var image = new Image { Source = new BitmapImage(new Uri("image.jpg", UriKind.Relative)) };
image.Effect = new Invert();
canvas.Children.Add(image);
```

---

## Sepia

Эффект сепии (коричневатый, винтажный тон).

### Описание

`Sepia` — это эффект, который придаёт изображению коричневато-жёлтый винтажный тон, часто используемый для имитации старых фотографий. Свойство `Factor` контролирует интенсивность эффекта.

### Основные свойства

| Свойство | Тип | Значение по умолчанию | Описание |
|----------|-----|----------------------|---------|
| **Input** | `Brush` | автоматически | Исходное изображение (устанавливается системой WPF) |
| **Factor** | `float` | `0.5f` | Интенсивность сепии (0.0 = оригинальные цвета, 1.0 = полная сепия) |

### Примеры использования

#### Полная сепия

```xaml
<Image Source="image.jpg">
    <Image.Effect>
        <effects:Sepia Factor="1.0" />
    </Image.Effect>
</Image>
```

#### Лёгкая сепия (50%)

```xaml
<Image Source="image.jpg">
    <Image.Effect>
        <effects:Sepia Factor="0.5" />
    </Image.Effect>
</Image>
```

#### Винтажная галерея фотографий

```xaml
<ItemsControl ItemsSource="{Binding PhotoCollection}">
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <Image Source="{Binding ImagePath}" Width="150" Height="150">
                <Image.Effect>
                    <effects:Sepia Factor="0.7" />
                </Image.Effect>
            </Image>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

#### В коде с анимацией

```csharp
var image = new Image { Source = new BitmapImage(new Uri("image.jpg", UriKind.Relative)) };
var sepia = new Sepia { Factor = 0.3f };
image.Effect = sepia;

// Анимация усиления сепии
var animation = new DoubleAnimation
{
    From = 0,
    To = 1,
    Duration = TimeSpan.FromSeconds(1),
    AutoReverse = true
};
sepia.BeginAnimation(Sepia.FactorProperty, animation);

canvas.Children.Add(image);
```

---

## Opacity

Контроль прозрачности элемента через шейдер.

### Описание

`Opacity` — это эффект, который контролирует прозрачность изображения на уровне пиксельного шейдера. Отличается от стандартного свойства `Opacity` WPF тем, что применяется как эффект.

### Основные свойства

| Свойство | Тип | Значение по умолчанию | Описание |
|----------|-----|----------------------|---------|
| **Input** | `Brush` | автоматически | Исходное изображение (устанавливается системой WPF) |
| **Factor** | `float` | `0.5f` | Степень видимости (0.0 = полностью прозрачный, 1.0 = полностью видимый) |

### Примеры использования

#### Полупрозрачное изображение

```xaml
<Image Source="image.jpg">
    <Image.Effect>
        <effects:Opacity Factor="0.5" />
    </Image.Effect>
</Image>
```

#### Как маска (плавное исчезновение)

```xaml
<Grid>
    <Image Source="image.jpg" x:Name="FadingImage">
        <Image.Effect>
            <effects:Opacity x:Name="OpacityEffect" Factor="1.0" />
        </Image.Effect>
        <Grid.Triggers>
            <EventTrigger RoutedEvent="Grid.MouseEnter">
                <BeginStoryboard>
                    <Storyboard>
                        <DoubleAnimation
                            Storyboard.TargetName="OpacityEffect"
                            Storyboard.TargetProperty="Factor"
                            To="0"
                            Duration="0:0:1" />
                    </Storyboard>
                </BeginStoryboard>
            </EventTrigger>
        </Grid.Triggers>
    </Image>
</Grid>
```

---

## ColorAlphaKey

Установка прозрачного цвета (подобно "ключу цвета" в видеомонтаже).

### Описание

`ColorAlphaKey` — это эффект, который делает определённый цвет прозрачным (альфа-ключ). Полезен для создания масок и удаления фонов определённого цвета.

### Основные свойства

| Свойство | Тип | Значение по умолчанию | Описание |
|----------|-----|----------------------|---------|
| **Input** | `Brush` | автоматически | Исходное изображение (устанавливается системой WPF) |

**Примечание:** Данный эффект не имеет свойства для выбора цвета в текущей реализации. Требуется расширение функциональности или использование напрямую через HLSL.

### Примеры использования

```xaml
<Image Source="image.jpg">
    <Image.Effect>
        <effects:ColorAlphaKey />
    </Image.Effect>
</Image>
```

---

## Рекомендации по использованию

### Выбор эффекта

| Сценарий | Рекомендуемый эффект |
|----------|---------------------|
| Размытие всего изображения | `Blur` |
| Имитация движения в одном направлении | `DirectionalBlur` |
| Эффект приближения/удаления | `ZoomBlur` |
| Чёрно-белое изображение с переходом | `GrayScale` |
| Винтажная фотография | `Sepia` |
| Чистое чёрно-белое | `BlackAndWhite` |
| Негатив изображения | `Invert` |
| Контроль прозрачности через шейдер | `Opacity` |
| Удаление цветного фона | `ColorAlphaKey` |

### Производительность

- **Лучшее:** Одиночные эффекты на одном элементе
- **Хорошее:** 5-10 элементов с эффектами на экране
- **Избегать:** Применение эффектов к сотням элементов одновременно
- Рекомендуется отключать эффекты при сворачивании окна

### Качество видеокарты

- **Требуется:** DirectX 9 или выше
- **Оптимально:** Современные GPU (2010+)
- **Может быть медленным:** Интегрированная графика на старых компьютерах

---

## Примеры

### Пример 1: Интерактивная галерея с эффектами

```xaml
<Window
    xmlns:effects="clr-namespace:MathCore.WPF.Shaders;assembly=MathCore.WPF">
    <Grid Background="Gray">
        <ItemsControl ItemsSource="{Binding Images}">
            <ItemsControl.ItemsPanel>
                <ItemsPanelTemplate>
                    <UniformGrid Columns="3" />
                </ItemsPanelTemplate>
            </ItemsControl.ItemsPanel>
            <ItemsControl.ItemTemplate>
                <DataTemplate>
                    <Grid Width="150" Height="150">
                        <Image Source="{Binding Path}" />
                        <Grid
                            Background="Black"
                            Opacity="0"
                            x:Name="HoverOverlay">
                            <Grid.Triggers>
                                <EventTrigger RoutedEvent="Grid.MouseEnter">
                                    <BeginStoryboard>
                                        <Storyboard>
                                            <DoubleAnimation
                                                Storyboard.TargetName="HoverOverlay"
                                                Storyboard.TargetProperty="Opacity"
                                                To="0.3"
                                                Duration="0:0:0.2" />
                                        </Storyboard>
                                    </BeginStoryboard>
                                </EventTrigger>
                            </Grid.Triggers>
                        </Grid>
                    </Grid>
                </DataTemplate>
            </ItemsControl.ItemTemplate>
        </ItemsControl>
    </Grid>
</Window>
```

### Пример 2: Анимированные эффекты в коде

```csharp
public class ImageEffectViewModel
{
    public void ApplyBlurEffect(Image image, double duration)
    {
        var blur = new Blur { Factor = 0 };
        image.Effect = blur;

        var animation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromSeconds(duration),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever
        };
        
        blur.BeginAnimation(Blur.FactorProperty, animation);
    }

    public void ApplyGrayScaleOnLoad(Image image)
    {
        image.Effect = new GrayScale { Factor = 1.0f };
        
        // Переход к цвету при наведении (нужен код в XAML)
    }

    public void ApplyVintageEffect(Image image)
    {
        image.Effect = new Sepia { Factor = 0.8f };
    }
}
```

### Пример 3: Комбинирование эффектов

```xaml
<!-- Примечание: WPF позволяет применять только один Effect за раз, 
     поэтому для комбинирования нужно использовать LayerStack или выполнять
     комбинирование эффектов в пользовательском шейдере -->

<!-- Вариант 1: Чередование эффектов -->
<Image Source="image.jpg" x:Name="MainImage" />

<!-- Вариант 2: Использование Grid с несколькими слоями -->
<Grid>
    <Image Source="image.jpg" Opacity="0.5">
        <Image.Effect>
            <effects:GrayScale Factor="1.0" />
        </Image.Effect>
    </Image>
    <Image Source="image.jpg">
        <Image.Effect>
            <effects:Sepia Factor="0.5" />
        </Image.Effect>
    </Image>
</Grid>
```

---

## Примечания

- Все эффекты выполняются на GPU, что делает их очень быстрыми
- Поддержка эффектов зависит от видеокарты и драйверов
- На некоторых системах эффекты могут быть недоступны; в этом случае элементы отрисовываются без эффектов
- Для максимальной производительности избегайте частого изменения свойств эффектов
- Используйте анимации вместо прямого изменения свойств в обработчиках событий
