# MathCore.WPF.Shapes

Модуль содержит набор расширенных WPF фигур и вспомогательных классов для работы с геометрическими фигурами в приложениях на Windows Presentation Foundation.

## Содержание

- [ShapeBase](#shapebase) — базовый класс для фигур
- [Arc](#arc) — дуга окружности
- [Pie](#pie) — сектор круга или кольца
- [Arrow](#arrow) — стрелка с настраиваемыми параметрами
- [LineEx](#lineex) — расширение для Line с поддержкой привязки к точкам
- [LinePoint](#linepoint) — вспомогательный класс для работы с точками линии
- [PointLine](#pointline) — линия с поддержкой точек

---

## ShapeBase

Абстрактный базовый класс для всех пользовательских фигур с поддержкой растягивания и измерения видимого прямоугольника.

### Описание

`ShapeBase` наследуется от `System.Windows.Shapes.Shape` и предоставляет:
- Вычисление видимого прямоугольника (`_VisibleRect`) с учётом толщины обводки
- Обработка различных режимов растягивания (`Stretch`)
- Переопределяемые методы для измерения и размещения фигуры

### Основные методы

#### `ArrangeOverride(Size)`

Переопределённый метод, который:
- Вычисляет видимый прямоугольник с учётом толщины обводки
- Применяет режим растягивания (`Stretch.None`, `Stretch.Fill`, `Stretch.Uniform`, `Stretch.UniformToFill`)

```csharp
protected override Size ArrangeOverride(Size FinalSize)
```

**Параметры:**
- `FinalSize` — конечный размер для расположения фигуры

**Возвращаемое значение:**
- `Size` — размер после расположения

#### `MeasureOverride(Size)`

Переопределённый метод для измерения фигуры:

```csharp
protected override Size MeasureOverride(Size ConstraintSize)
```

**Параметры:**
- `ConstraintSize` — размер ограничения для измерения

---

## Arc

Фигура WPF для рисования дуги окружности с поддержкой эллиптических дуг.

### Описание

`Arc` — это фигура, которая рисует дугу с заданными начальным и конечным углами. Дуга может быть части эллипса, благодаря независимым радиусам по осям X и Y (заложено в прямоугольнике отрисовки).

### Основные свойства

| Свойство | Тип | Значение по умолчанию | Описание |
|----------|-----|----------------------|---------|
| **R** | `double` | `1.0` | Радиус дуги в относительных единицах от 0 до 1. Значение интерпретируется относительно размеров контейнера. При значении 1 дуга строится по максимальному доступному радиусу в пределах прямоугольника |
| **StartAngle** | `double` | `0.0` | Начальный угол дуги в градусах. Отсчёт ведётся по часовой стрелке, 0° направлен вправо, 90° вниз |
| **StopAngle** | `double` | `360.0` | Конечный угол дуги в градусах. После нормализации обоих углов дуга рисуется по часовой стрелке от StartAngle к StopAngle |

### Примеры использования

#### Полная окружность

```xaml
<Window
    xmlns:shapes="clr-namespace:MathCore.WPF.Shapes;assembly=MathCore.WPF">
    <Canvas>
        <shapes:Arc
            Canvas.Left="10"
            Canvas.Top="10"
            Width="100"
            Height="100"
            Stroke="Blue"
            StrokeThickness="2"
            R="1"
            StartAngle="0"
            StopAngle="360" />
    </Canvas>
</Window>
```

#### Полукруг

```xaml
<shapes:Arc
    Width="100"
    Height="100"
    Stroke="Red"
    StrokeThickness="3"
    R="0.8"
    StartAngle="0"
    StopAngle="180" />
```

#### Четверть окружности с внутренним радиусом

```xaml
<shapes:Arc
    Width="150"
    Height="150"
    Stroke="Green"
    StrokeThickness="2"
    R="0.75"
    StartAngle="45"
    StopAngle="135" />
```

#### С привязкой в коде

```csharp
var arc = new Arc
{
    Width = 100,
    Height = 100,
    R = 1,
    StartAngle = 0,
    StopAngle = 270,
    Stroke = Brushes.Purple,
    StrokeThickness = 2
};

// Привязка углов к свойствам ViewModel
var startBinding = new Binding("StartAngleValue");
arc.SetBinding(Arc.StartAngleProperty, startBinding);

var stopBinding = new Binding("StopAngleValue");
arc.SetBinding(Arc.StopAngleProperty, stopBinding);
```

### Особенности

- Дуга всегда рисуется **по часовой стрелке** от нормализованного `StartAngle` к нормализованному `StopAngle`
- Углы нормализуются к диапазону `[0°; 360°)`
- Значения радиуса `R` вне диапазона `[0; 1]` автоматически ограничиваются
- Очень малые дуги (менее `1e-6` градусов) игнорируются
- Окружность строится при `|StopAngle - StartAngle| >= 360 - MinArcDegrees`

### Примеры углов

```
StartAngle=270, StopAngle=60   → дуга 150° по часовой (270→360→60)
StartAngle=60,  StopAngle=270  → дуга 210° по часовой (60→270)
StartAngle=0,   StopAngle=360  → полная окружность
StartAngle=90,  StopAngle=90   → ничего не рисуется (дуга 0°)
```

---

## Pie

Фигура для рисования сектора круга или кольца (сектора с внутренним радиусом).

### Описание

`Pie` — это фигура, которая рисует сектор с поддержкой как полного сектора (от центра), так и кольцевого сектора (с внутренним радиусом). Сектор может быть использован для создания круговых диаграмм, кольцевых диаграмм (доннатов) и других визуализаций.

### Основные свойства

| Свойство | Тип | Значение по умолчанию | Описание |
|----------|-----|----------------------|---------|
| **OuterRadius** | `double` | `1.0` | Внешний радиус сектора в относительных единицах от 0 до 1 |
| **InnerRadius** | `double` | `0.0` | Внутренний радиус сектора в относительных единицах от 0 до 1. При значении 0 сектор рисуется от центра (типичный сектор). При значении > 0 рисуется кольцевой сектор |
| **StartAngle** | `double` | `0.0` | Начальный угол сектора в градусах (отсчёт по часовой стрелке) |
| **StopAngle** | `double` | `360.0` | Конечный угол сектора в градусах |
| **Angle** | `double` | `360.0` | Угол раствора сектора в градусах (вычисляется как `StopAngle - StartAngle`) |
| **IsAligned** | `bool` | `false` | Если `true`, сектор выравнивается по меньшему размеру контейнера (квадрат) |

### Примеры использования

#### Сектор пиццы (45°)

```xaml
<shapes:Pie
    Width="200"
    Height="200"
    Fill="LightBlue"
    Stroke="Navy"
    StrokeThickness="2"
    OuterRadius="1"
    InnerRadius="0"
    StartAngle="0"
    StopAngle="45" />
```

#### Кольцевой сектор (донат)

```xaml
<shapes:Pie
    Width="200"
    Height="200"
    Fill="Orange"
    Stroke="DarkOrange"
    StrokeThickness="1"
    OuterRadius="1"
    InnerRadius="0.6"
    StartAngle="45"
    Angle="90" />
```

#### Круговая диаграмма (элемент)

```xaml
<shapes:Pie
    Width="200"
    Height="200"
    Fill="LimeGreen"
    Stroke="Green"
    StrokeThickness="1"
    OuterRadius="1"
    InnerRadius="0"
    StartAngle="90"
    Angle="120"
    IsAligned="True" />
```

#### С привязкой данных

```csharp
var pie = new Pie
{
    Width = 200,
    Height = 200,
    Fill = Brushes.SkyBlue,
    Stroke = Brushes.Navy,
    StrokeThickness = 2,
    OuterRadius = 1,
    InnerRadius = 0.5
};

// Привязка углов к свойствам ViewModel
var binding = new Binding("PieAngle");
pie.SetBinding(Pie.AngleProperty, binding);

var startBinding = new Binding("StartAngle");
pie.SetBinding(Pie.StartAngleProperty, startBinding);
```

### Особенности

- Сектор рисуется **по часовой стрелке** от нормализованного `StartAngle` к нормализованному `StopAngle`
- При `InnerRadius = 0` рисуется типичный сектор круга (от центра)
- При `InnerRadius > 0` рисуется кольцевой сектор (донат, сегмент кольца)
- Система автоматически гарантирует `OuterRadius >= InnerRadius`
- Очень малые углы игнорируются
- Свойство `Angle` связано с `StopAngle`: `StopAngle = StartAngle + Angle`

### Пример использования в коде

```csharp
// Создание круговой диаграммы с четырьмя сегментами
var colors = new[] { Brushes.Red, Brushes.Yellow, Brushes.Green, Brushes.Blue };
var percentages = new[] { 25, 25, 30, 20 };

double startAngle = 0;
foreach (var percentage in percentages)
{
    var pie = new Pie
    {
        Width = 300,
        Height = 300,
        Fill = colors[Array.IndexOf(percentages, percentage)],
        OuterRadius = 1,
        InnerRadius = 0.3, // Кольцевая диаграмма
        StartAngle = startAngle,
        Angle = percentage * 3.6 // Преобразование процентов в градусы
    };
    
    canvas.Children.Add(pie);
    startAngle += percentage * 3.6;
}
```

---

## Arrow

Визуальный элемент стрелки с настраиваемыми параметрами линии и головы.

### Описание

`Arrow` — это фигура, которая рисует стрелку, состоящую из линии и треугольной головы. Поддерживает полную настройку координат, размеров головы, стиля линии и заливки.

### Основные свойства

| Свойство | Тип | Значение по умолчанию | Описание |
|----------|-----|----------------------|---------|
| **X1** | `double` | `0.0` | X-координата начальной точки линии стрелки |
| **Y1** | `double` | `0.0` | Y-координата начальной точки линии стрелки |
| **X2** | `double` | `0.0` | X-координата конечной точки линии стрелки |
| **Y2** | `double` | `0.0` | Y-координата конечной точки линии стрелки |
| **ArrowHeadWidth** | `double` | `10.0` | Ширина головы стрелки (ширина треугольника в основании) |
| **ArrowHeadLength** | `double` | `15.0` | Длина головы стрелки (высота треугольника) |
| **ArrowHeadOffset** | `double` | `0.0` | Отступ между концом линии и основанием головы стрелки |
| **IsArrowHeadClosed** | `bool` | `true` | Замкнут ли контур головы стрелки (заливка треугольника) |

### Примеры использования

#### Простая стрелка

```xaml
<shapes:Arrow
    X1="10" Y1="10"
    X2="100" Y2="100"
    ArrowHeadWidth="10"
    ArrowHeadLength="15"
    Stroke="Blue"
    StrokeThickness="2"
    Fill="LightBlue" />
```

#### Горизонтальная стрелка с большой головой

```xaml
<shapes:Arrow
    X1="50" Y1="50"
    X2="250" Y2="50"
    ArrowHeadWidth="20"
    ArrowHeadLength="25"
    ArrowHeadOffset="5"
    Stroke="Red"
    StrokeThickness="3"
    Fill="Red" />
```

#### Открытая стрелка (без заливки головы)

```xaml
<shapes:Arrow
    X1="30" Y1="30"
    X2="150" Y2="150"
    ArrowHeadWidth="12"
    ArrowHeadLength="18"
    IsArrowHeadClosed="False"
    Stroke="Green"
    StrokeThickness="2"
    Fill="Transparent" />
```

#### С привязкой данных

```csharp
var arrow = new Arrow
{
    ArrowHeadWidth = 12,
    ArrowHeadLength = 16,
    ArrowHeadOffset = 2,
    IsArrowHeadClosed = true,
    Stroke = Brushes.Navy,
    StrokeThickness = 2,
    Fill = Brushes.SkyBlue
};

// Привязка координат к свойствам ViewModel
var x1Binding = new Binding("StartPoint.X");
var y1Binding = new Binding("StartPoint.Y");
var x2Binding = new Binding("EndPoint.X");
var y2Binding = new Binding("EndPoint.Y");

arrow.SetBinding(Arrow.X1Property, x1Binding);
arrow.SetBinding(Arrow.Y1Property, y1Binding);
arrow.SetBinding(Arrow.X2Property, x2Binding);
arrow.SetBinding(Arrow.Y2Property, y2Binding);
```

### Особенности

- Стрелка состоит из линии (от (X1, Y1) к конечной точке) и треугольной головы
- Глава автоматически ориентируется в направлении линии
- Параметры `ArrowHeadWidth`, `ArrowHeadLength`, `ArrowHeadOffset` не могут быть отрицательными
- При очень малой длине линии стрелка может не отрисоваться корректно
- `IsArrowHeadClosed = true` означает, что голова заливается указанным цветом (`Fill`)
- `IsArrowHeadClosed = false` означает, что голова рисуется только контуром

---

## LineEx

Статический вспомогательный класс с присоединяемыми свойствами для работы с точками линии (`System.Windows.Shapes.Line`).

### Описание

`LineEx` предоставляет присоединяемые свойства `P1` и `P2` для привязки начальной и конечной точек линии напрямую к моделям-представлениям (ViewModel). Обеспечивает автоматическую синхронизацию между точками и координатами `X1`, `Y1`, `X2`, `Y2`.

### Основные присоединяемые свойства

| Свойство | Тип | Описание |
|----------|-----|---------|
| **P1** | `Point` | Начальная точка линии. Привязывается к свойству ViewModel, заменяя необходимость в привязке координат X1/Y1 |
| **P2** | `Point` | Конечная точка линии. Привязывается к свойству ViewModel, заменяя необходимость в привязке координат X2/Y2 |

### Примеры использования

#### Привязка через точки (рекомендуется)

```xaml
<shapes:Line
    local:LineEx.P1="{Binding StartPoint}"
    local:LineEx.P2="{Binding EndPoint}"
    Stroke="Blue"
    StrokeThickness="2" />
```

#### Привязка отдельных координат (альтернатива)

```xaml
<shapes:Line
    X1="{Binding StartPoint.X}"
    Y1="{Binding StartPoint.Y}"
    X2="{Binding EndPoint.X}"
    Y2="{Binding EndPoint.Y}"
    Stroke="Blue"
    StrokeThickness="2" />
```

#### В коде

```csharp
var line = new Line();

// Привязка через присоединяемые свойства
var p1Binding = new Binding("StartPoint");
var p2Binding = new Binding("EndPoint");

LineEx.SetP1(line, p1Binding);
LineEx.SetP2(line, p2Binding);

// Или установка значений напрямую
LineEx.SetP1(line, new Point(10, 10));
LineEx.SetP2(line, new Point(100, 100));
```

### Особенности

- Использует `ConditionalWeakTable` для хранения вспомогательных объектов, предотвращая утечку памяти
- Автоматически синхронизирует точки с координатами при изменении через привязку
- При прямом изменении координат в коде точки могут не обновляться
- Оптимизирован для использования в шаблонах элементов (`ItemsControl`, `ListBox` и т.д.)

---

## LinePoint

Статический вспомогательный класс для работы с присоединяемыми свойствами для начальной и конечной точек линии с использованием слабых ссылок.

### Описание

`LinePoint` предоставляет альтернативный механизм присоединяемых свойств для синхронизации точек линии, использующий список слабых ссылок для отслеживания привязанных линий.

### Основные методы

- **IsStartAttached(Line)** — проверяет, привязана ли линия к начальной точке
- **IsEndAttached(Line)** — проверяет, привязана ли линия к конечной точке

### Примечание

Данный класс используется внутри фреймворка для отслеживания привязанных элементов и управления жизненным циклом привязок. Обычно разработчики используют `LineEx` для работы с точками линии.

---

## PointLine

Фигура WPF для рисования линии с поддержкой работы с точками (альтернатива стандартной `System.Windows.Shapes.Line`).

### Описание

`PointLine` — это расширение стандартной линии WPF с дополнительной поддержкой свойств `Start` и `End` для удобной работы с точками.

### Основные свойства

| Свойство | Тип | Значение по умолчанию | Описание |
|----------|-----|----------------------|---------|
| **Start** | `Point` | `(0, 0)` | Начальная точка линии |
| **End** | `Point` | `(0, 0)` | Конечная точка линии |
| **X1** | `double` | `0.0` | X-координата начальной точки (связана со свойством `Start`) |
| **Y1** | `double` | `0.0` | Y-координата начальной точки (связана со свойством `Start`) |
| **X2** | `double` | `0.0` | X-координата конечной точки (связана со свойством `End`) |
| **Y2** | `double` | `0.0` | Y-координата конечной точки (связана со свойством `End`) |

### Примеры использования

#### Через точки

```xaml
<shapes:PointLine
    Start="10,10"
    End="100,100"
    Stroke="Blue"
    StrokeThickness="2" />
```

#### Через координаты

```xaml
<shapes:PointLine
    X1="20" Y1="30"
    X2="150" Y2="120"
    Stroke="Red"
    StrokeThickness="3" />
```

#### С привязкой в коде

```csharp
var line = new PointLine
{
    Stroke = Brushes.Green,
    StrokeThickness = 2
};

var startBinding = new Binding("StartPoint");
var endBinding = new Binding("EndPoint");

line.SetBinding(PointLine.StartProperty, startBinding);
line.SetBinding(PointLine.EndProperty, endBinding);
```

### Особенности

- Koordinаты `X1`, `Y1`, `X2`, `Y2` автоматически синхронизируются с точками `Start` и `End`
- При изменении любой из координат, соответствующая точка обновляется
- Проще в использовании, чем стандартная `Line`, если вы работаете с точками

---

## Рекомендации по использованию

### Выбор между Arc, Pie и стандартной Ellipse

| Сценарий | Рекомендуемая фигура |
|----------|---------------------|
| Рисование полной окружности | `Arc` с `StartAngle=0, StopAngle=360` или стандартная `Ellipse` |
| Рисование дуги | `Arc` |
| Рисование сектора круга | `Pie` с `InnerRadius=0` |
| Рисование кольцевого сектора (донат) | `Pie` с `InnerRadius > 0` |
| Круговая диаграмма | Несколько `Pie` с разными `StartAngle` и `Angle` |

### Выбор между Line, LineEx и PointLine

| Сценарий | Рекомендуемая фигура |
|----------|---------------------|
| Простая линия без привязки | Стандартная `Line` |
| Привязка к ViewModel через отдельные координаты | Стандартная `Line` с привязкой X1, Y1, X2, Y2 |
| Привязка к ViewModel через точки | `LineEx` с `P1` и `P2` |
| Работа с точками в коде | `PointLine` |

### Производительность

- `Arc`, `Pie` и `Arrow` используют `StreamGeometry` для оптимальной производительности
- Все фигуры замораживаются (`Freeze()`) после создания для улучшения производительности
- При необходимости рисования большого количества фигур рассмотрите использование `DrawingContext` напрямую

---

## Пример: Создание круговой диаграммы

```xaml
<Window
    xmlns:shapes="clr-namespace:MathCore.WPF.Shapes;assembly=MathCore.WPF">
    <Canvas>
        <!-- Красный сектор (30%) -->
        <shapes:Pie
            Canvas.Left="50" Canvas.Top="50"
            Width="200" Height="200"
            Fill="Red" Stroke="DarkRed" StrokeThickness="1"
            OuterRadius="1" InnerRadius="0.3"
            StartAngle="0" Angle="108" />
        
        <!-- Жёлтый сектор (20%) -->
        <shapes:Pie
            Canvas.Left="50" Canvas.Top="50"
            Width="200" Height="200"
            Fill="Gold" Stroke="Orange" StrokeThickness="1"
            OuterRadius="1" InnerRadius="0.3"
            StartAngle="108" Angle="72" />
        
        <!-- Зелёный сектор (30%) -->
        <shapes:Pie
            Canvas.Left="50" Canvas.Top="50"
            Width="200" Height="200"
            Fill="Green" Stroke="DarkGreen" StrokeThickness="1"
            OuterRadius="1" InnerRadius="0.3"
            StartAngle="180" Angle="108" />
        
        <!-- Синий сектор (20%) -->
        <shapes:Pie
            Canvas.Left="50" Canvas.Top="50"
            Width="200" Height="200"
            Fill="Blue" Stroke="Navy" StrokeThickness="1"
            OuterRadius="1" InnerRadius="0.3"
            StartAngle="288" Angle="72" />
    </Canvas>
</Window>
```

---

## Примечания

- Все фигуры наследуются от `System.Windows.Shapes.Shape` и поддерживают стандартные свойства WPF: `Stroke`, `StrokeThickness`, `Fill`, `Stretch` и т.д.
- Угловые свойства (`StartAngle`, `StopAngle`, `Angle`) работают в градусах
- Углы нормализуются внутри фигур, поэтому допускаются значения вне диапазона [0°; 360°)
- Все геометрии замораживаются после создания для оптимизации производительности
