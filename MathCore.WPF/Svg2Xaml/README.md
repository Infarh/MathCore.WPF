# SVG2Xaml - Механизм рендеринга SVG для WPF

## Обзор

`SVG2Xaml` — это мощный механизм для загрузки и преобразования SVG (Scalable Vector Graphics) документов в WPF `DrawingImage` объекты. Он позволяет отображать масштабируемую векторную графику в приложениях WPF с полной поддержкой трансформаций, эффектов и стилей.

## Архитектура

Механизм состоит из следующих основных компонентов:

### Основные классы

| Класс | Назначение |
|-------|-----------|
| **SvgReader** | Статический класс для загрузки SVG документов из потоков и XML читателей |
| **SvgDocument** | Внутренний класс, представляющий структуру загруженного SVG документа |
| **SvgReaderOptions** | Класс для настройки параметров парсинга и рендеринга |
| **SvgBaseElement** | Базовый класс для всех элементов SVG |
| **SvgDrawableBaseElement** | Базовый класс для отрисовываемых элементов с поддержкой стилей |
| **SvgSvgElement** | Представляет корневой элемент `<svg>` |

### Иерархия элементов

```
SvgBaseElement
├── SvgDrawableBaseElement
│   ├── SvgSvgElement (корневой элемент)
│   ├── SvgRectElement (прямоугольник)
│   ├── SvgCircleElement (круг)
│   ├── SvgEllipseElement (эллипс)
│   ├── SvgLineElement (линия)
│   ├── SvgPathElement (путь)
│   ├── SvgPolygonElement (полигон)
│   └── [и другие элементы...]
└── [Вспомогательные элементы]
    ├── SvgDefsElement (определения)
    ├── SvgClipPathElement (обрезка)
    ├── SvgFilterElement (фильтры)
    └── [и другие...]
```

## Основные принципы использования

### 1. Загрузка SVG из файла

```csharp
using (var fileStream = File.OpenRead("image.svg"))
{
    var drawingImage = SvgReader.Load(fileStream);
    // Использование drawingImage в WPF контролах
}
```

### 2. Загрузка SVG из потока с опциями

```csharp
var options = new SvgReaderOptions 
{ 
    IgnoreEffects = true  // Отключить применение фильтр-эффектов
};

using (var stream = File.OpenRead("image.svg"))
{
    var drawingImage = SvgReader.Load(stream, options);
}
```

### 3. Загрузка SVG через XmlReader

```csharp
var settings = new XmlReaderSettings 
{ 
    DtdProcessing = DtdProcessing.Ignore 
};

using (var xmlReader = XmlReader.Create("image.svg", settings))
{
    var drawingImage = SvgReader.Load(xmlReader);
}
```

### 4. Использование DrawingImage в XAML

```xaml
<Image Source="{Binding SvgDrawingImage, Mode=OneTime}" 
       Width="200" 
       Height="200" />
```

Соответствующий C# код:

```csharp
public DrawingImage SvgDrawingImage { get; private set; }

public void LoadSvgImage(string filePath)
{
    using (var stream = File.OpenRead(filePath))
    {
        SvgDrawingImage = SvgReader.Load(stream);
    }
}
```

## Поддерживаемые элементы SVG

### Основные графические элементы

- `<svg>` — корневой элемент
- `<g>` — группировка элементов
- `<rect>` — прямоугольник
- `<circle>` — круг
- `<ellipse>` — эллипс
- `<line>` — линия
- `<polyline>` — ломаная линия
- `<polygon>` — полигон
- `<path>` — путь (использует команды M, L, C, Z и т.д.)
- `<text>` — текст
- `<image>` — встроенное изображение
- `<use>` — использование определённых элементов

### Элементы определений и стилей

- `<defs>` — определения элементов
- `<style>` — стили CSS
- `<linearGradient>` — линейный градиент
- `<radialGradient>` — радиальный градиент
- `<pattern>` — паттерн
- `<marker>` — маркер для линий
- `<mask>` — маска
- `<clipPath>` — обрезка

### Фильтры и эффекты

- `<filter>` — фильтр
- `<feGaussianBlur>` — размытие
- `<feBlend>` — смешивание
- `<feColorMatrix>` — матрица цветов

## Поддерживаемые атрибуты стиля

### Заполнение и обводка

| Атрибут | Описание | Пример |
|---------|---------|--------|
| `fill` | Цвет или градиент заполнения | `fill="#FF0000"` |
| `fill-opacity` | Прозрачность заполнения (0-1) | `fill-opacity="0.5"` |
| `stroke` | Цвет обводки | `stroke="#000000"` |
| `stroke-width` | Толщина обводки | `stroke-width="2"` |
| `stroke-opacity` | Прозрачность обводки | `stroke-opacity="0.8"` |
| `stroke-linecap` | Форма окончания линии (butt, round, square) | `stroke-linecap="round"` |
| `stroke-linejoin` | Способ соединения (miter, round, bevel) | `stroke-linejoin="bevel"` |
| `stroke-dasharray` | Штриховка линии | `stroke-dasharray="5,5"` |

### Трансформации

| Атрибут | Описание |
|---------|---------|
| `transform` | Трансформация элемента (translate, rotate, scale, skew) |

### Видимость и клиппинг

| Атрибут | Описание |
|---------|---------|
| `opacity` | Общая прозрачность элемента |
| `clip-path` | Ссылка на элемент обрезки |
| `mask` | Ссылка на маску |
| `display` | Видимость (inline, none и т.д.) |
| `filter` | Ссылка на фильтр эффектов |

## Примеры использования

### Пример 1: Простое отображение SVG

```csharp
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoadSvgImage("Resources/icon.svg");
    }

    private void LoadSvgImage(string filePath)
    {
        using (var stream = File.OpenRead(filePath))
        {
            var drawingImage = SvgReader.Load(stream);
            SvgImage.Source = drawingImage;
        }
    }
}
```

XAML:

```xaml
<Window x:Class="SvgDemo.MainWindow" ...>
    <Grid>
        <Image x:Name="SvgImage" 
               Width="300" 
               Height="300" />
    </Grid>
</Window>
```

### Пример 2: Загрузка SVG из ресурсов

```csharp
public static DrawingImage LoadSvgFromResource(string resourceName)
{
    var assembly = Assembly.GetExecutingAssembly();
    using (var stream = assembly.GetManifestResourceStream(resourceName))
    {
        if (stream == null)
            throw new ArgumentException($"Ресурс '{resourceName}' не найден");
        
        return SvgReader.Load(stream);
    }
}

// Использование:
var svgImage = LoadSvgFromResource("MyApp.Resources.logo.svg");
imageControl.Source = svgImage;
```

### Пример 3: Настройка опций рендеринга

```csharp
public DrawingImage LoadSvgWithOptions(string filePath, bool ignoreEffects = false)
{
    var options = new SvgReaderOptions 
    { 
        IgnoreEffects = ignoreEffects 
    };

    using (var stream = File.OpenRead(filePath))
    {
        return SvgReader.Load(stream, options);
    }
}

// Загрузка с отключением эффектов для улучшения производительности
var image = LoadSvgWithOptions("Resources/complex.svg", ignoreEffects: true);
```

### Пример 4: Обработка ошибок

```csharp
public DrawingImage TryLoadSvg(string filePath)
{
    try
    {
        using (var stream = File.OpenRead(filePath))
        {
            return SvgReader.Load(stream);
        }
    }
    catch (FileNotFoundException)
    {
        MessageBox.Show("SVG файл не найден");
        return null;
    }
    catch (XmlException ex)
    {
        MessageBox.Show($"Ошибка парсинга XML: {ex.Message}");
        return null;
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка загрузки SVG: {ex.Message}");
        return null;
    }
}
```

## Производительность

### Рекомендации для оптимизации

1. **Отключение эффектов** — если фильтр-эффекты не требуются:
   ```csharp
   var options = new SvgReaderOptions { IgnoreEffects = true };
   ```

2. **Кэширование** — кэшируйте загруженные DrawingImage объекты:
   ```csharp
   private static readonly Dictionary<string, DrawingImage> SvgCache = [];

   public DrawingImage GetSvg(string filePath)
   {
       if (!SvgCache.TryGetValue(filePath, out var cached))
       {
           using (var stream = File.OpenRead(filePath))
           {
               cached = SvgReader.Load(stream);
               SvgCache[filePath] = cached;
           }
       }
       return cached;
   }
   ```

3. **Асинхронная загрузка** — для больших SVG файлов:
   ```csharp
   public async Task<DrawingImage> LoadSvgAsync(string filePath)
   {
       return await Task.Run(() => 
       {
           using (var stream = File.OpenRead(filePath))
           {
               return SvgReader.Load(stream);
           }
       });
   }
   ```

## Лицензия

Данный механизм основан на Svg2Xaml и распространяется по лицензии GNU Lesser General Public License v3 или выше.

Оригинальный авторский проект: Svg2Xaml (Boris Richter)
Интеграция и развитие: MathCore.WPF

## Ограничения

1. **Шрифты** — использование нестандартных шрифтов может быть ограничено доступными шрифтами системы
2. **JavaScript** — не поддерживается
3. **Некоторые CSS свойства** — могут игнорироваться
4. **Анимация** — не поддерживается (нужно использовать WPF анимации)
5. **Интерактивность** — события клика и наведения требуют дополнительной реализации

## Диагностика проблем

### SVG не отображается

1. Убедитесь что файл существует и путь указан корректно
2. Проверьте что SVG имеет корректное пространство имён: `xmlns="http://www.w3.org/2000/svg"`
3. Используйте отладчик для проверки исключений:
   ```csharp
   try 
   { 
       var img = SvgReader.Load(stream); 
   } 
   catch (XmlException ex) 
   { 
       Debug.WriteLine($"Ошибка: {ex.Message}"); 
   }
   ```

### Элемент не рисуется

1. Проверьте атрибуты `fill` и `stroke`
2. Убедитесь что размеры элемента больше 0
3. Проверьте `opacity` и `fill-opacity`

### Производительность низкая

1. Отключите эффекты через `SvgReaderOptions`
2. Используйте кэширование для часто используемых SVG
3. Рассмотрите использование более простых SVG с меньшим количеством путей

## Дополнительные ресурсы

- [W3C SVG Specification](https://www.w3.org/TR/SVG/)
- [MDN SVG Documentation](https://developer.mozilla.org/en-US/docs/Web/SVG)
- [WPF Graphics and Multimedia](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/)
