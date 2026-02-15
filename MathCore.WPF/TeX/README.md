# MathCore.WPF.TeX - TeX Rendering Engine для WPF

Модуль `MathCore.WPF.TeX` представляет собой полнофункциональный механизм рендеринга математических формул в формате TeX для приложений WPF.

## Основные возможности

- **Парсинг TeX формул** - поддержка синтаксиса LaTeX математических выражений
- **Рендеринг в WPF** - отрисовка формул в DrawingContext с поддержкой масштабирования
- **Гибкая система шрифтов** - поддержка различных TeX шрифтов и стилей
- **Управление стилями** - применение различных математических стилей (Display, Text, Script, ScriptScript)
- **Поддержка символов** - включение специальных математических символов и операторов
- **Визуальный контроль MathView** - WPF элемент для отображения формул в приложениях

## Архитектура модуля

### Основные компоненты

#### 1. **Парсер (TexFormulaParser)**
Преобразует строковое представление TeX формулы в дерево атомов (Atom).

```csharp
var parser = new TexFormulaParser();
var formula = parser.Parse("x^2 + y^2 = z^2");
```

#### 2. **Атомы (Atom и его наследники)**
Наименьшие единицы TeX формулы. Каждый атом отвечает за отрисовку своей части:

- `CharAtom` - отдельный символ
- `RowAtom` - горизонтальный ряд атомов
- `FractionAtom` - дробь
- `ScriptsAtom` - надстрочные/подстрочные индексы
- `BigOperatorAtom` - большие операторы (∑, ∫ и т.д.)
- `RootAtom` - корень (квадратный, кубический и т.д.)
- `FencedAtom` - огороженные выражения

#### 3. **Формула (TexFormula)**
Контейнер для корневого атома и управления процессом рендеринга.

```csharp
var formula = new TexFormula();
formula.Add(new CharAtom('x'));
var renderer = formula.GetRenderer(TexStyle.Display, 20.0);
```

#### 4. **Среда (TexEnvironment)**
Хранит контекстную информацию о текущем стиле, шрифте и прочие параметры рендеринга.

#### 5. **Шрифты (ITeXFont и наследники)**
Интерфейс для работы с TeX шрифтами:
- `DefaultTexFont` - стандартный TeX шрифт
- Обеспечивают метрики символов и сведения о расположении в шрифтах

#### 6. **Боксы (Box и наследники)**
Результат рендеринга атома. Содержат информацию о размерах и способны рисовать себя:

- `CharBox` - отрисовка символа
- `HorizontalBox` - горизонтальный композитный бокс
- `VerticalBox` - вертикальный композитный бокс
- `FractionBox` - отрисовка дроби
- `OverUnderBox` - элементы над/под выражением

#### 7. **Рендерер (TexRenderer)**
Финальный этап: преобразование дерева боксов в команды отрисовки WPF.

```csharp
var renderer = formula.GetRenderer(TexStyle.Display, scale: 20.0);
renderer.Render(drawingContext, x: 10, y: 10);
```

#### 8. **Визуальный контроль (MathView)**
WPF элемент для простого использования в XAML приложениях:

```xaml
<math:MathView Formula="x^2 + y^2 = z^2" Scale="24" Foreground="Black"/>
```

### Основной цикл обработки

```
Строка TeX
    ↓
TexFormulaParser.Parse()
    ↓
Дерево Atom объектов
    ↓
Atom.CreateBox(TexEnvironment)
    ↓
Дерево Box объектов
    ↓
Box.Draw(DrawingContext, scale, x, y)
    ↓
Визуализация в WPF
```

## Использование

### Базовое использование - парсинг и рендеринг

```csharp
// 1. Создать парсер
var parser = new TexFormulaParser();

// 2. Распарсить формулу
var formula = parser.Parse("\\frac{a}{b}");

// 3. Получить рендерер
var renderer = formula.GetRenderer(TexStyle.Display, scale: 20.0);

// 4. Отрисовать в DrawingContext
using (var drawingGroup = new DrawingGroup())
{
    var drawingContext = drawingGroup.Open();
    renderer.Render(drawingContext, x: 10, y: 10);
    drawingContext.Close();
}
```

### Использование в XAML (MathView)

```xaml
<Window xmlns:math="clr-namespace:MathCore.WPF.TeX">
    <StackPanel>
        <math:MathView 
            Formula="E = mc^2" 
            Scale="24" 
            Foreground="DarkBlue"
            Background="White"/>
    </StackPanel>
</Window>
```

### Программное построение формул

```csharp
var formula = new TexFormula();
formula.Add(new CharAtom('x'));
formula.Add(new ScriptsAtom(null, new CharAtom('2'), null, null));

var renderer = formula.GetRenderer(TexStyle.Text, 16.0);
```

## Поддерживаемые TeX команды

### Базовые команды
- `\frac{numbers}{denominator}` - обыкновенная дробь
- `\sqrt[n]{x}` - корень n-й степени
- `^` - надстрочный индекс (верхний индекс)
- `_` - подстрочный индекс (нижний индекс)
- `'` - штрих (производная)

### Стили и оформление
- `\overline{x}` - надчеркивание
- `\underline{x}` - подчеркивание
- `\hat{x}`, `\tilde{x}`, `\bar{x}` - диакритические знаки
- `\phantom{x}` - невидимый символ с тем же размером
- `\left` и `\right` - автоматическое масштабирование ограничителей

### Большие операторы
- `\sum` - сумма
- `\prod` - произведение
- `\int` - интеграл
- `\oint` - циркулярный интеграл

### Ограничители и скобки
- `(`, `)` - круглые скобки
- `[`, `]` - квадратные скобки
- `\{`, `\}` - фигурные скобки
- `|` - вертикальная линия
- `\|` - двойная вертикальная линия

## Стили математического текста

Модуль поддерживает четыре основных стиля TeX:

- **Display** (`TexStyle.Display`) - для отдельных строк формул
- **Text** (`TexStyle.Text`) - для формул внутри текста
- **Script** (`TexStyle.Script`) - для индексов (меньший размер)
- **ScriptScript** (`TexStyle.ScriptScript`) - для вложенных индексов (еще меньше)

Каждый стиль автоматически масштабирует шрифты согласно TeX стандартам.

## Конфигурация шрифтов

Модуль использует конфигурацию шрифтов из XML файлов:
- `TexFormulaSettings.xml` - основные параметры форматирования
- `TexSymbols.xml` - отображение символов на шрифты
- `GlueSettings.xml` - параметры пробелов между элементами
- `DefaultTexFont.xml` - параметры шрифта по умолчанию
- `PredefinedTexFormulas.xml` - предопределённые формулы

Все файлы конфигурации расположены в директории `Styles`.

## Расширение функциональности

### Добавление новых символов

1. Обновить `TexSymbols.xml` для добавления отображения символа на шрифт
2. Обновить `DefaultTexFont.xml` с метриками нового символа

### Добавление новых команд

1. Создать новый класс, наследующий `Atom`
2. Реализовать метод `CreateBox(TexEnvironment environment)`
3. Зарегистрировать команду в `TexFormulaParser`

## Производительность и оптимизация

- **Кэширование** - результаты парсинга и рендеринга кэшируются где возможно
- **Ленивое вычисление** - размеры боксов вычисляются по необходимости
- **Масштабирование вместо перерисовки** - используется коэффициент масштабирования вместо пересоздания боксов

## Обработка ошибок

Модуль выбрасывает следующие исключения:
- `TexParseException` - ошибка парсинга формулы
- `FormulaNotFoundException` - неизвестная команда
- `SymbolNotFoundException` - неизвестный символ
- `DelimiterMappingNotFoundException` - неизвестный ограничитель
- `SymbolMappingNotFoundException` - символ не может быть отображён на шрифт

## Примеры сложных формул

```
E = mc^2
\frac{-b \pm \sqrt{b^2 - 4ac}}{2a}
\int_0^\infty e^{-x^2} dx
\left(\frac{n}{k}\right) = \frac{n!}{k!(n-k)!}
```

## Лицензия и авторство

Данный модуль является частью проекта MathCore.WPF. Основан на идеях и компонентах Math.NET Numerics и WPF-Math.

## Дополнительные ресурсы

- Синтаксис TeX: https://en.wikibooks.org/wiki/LaTeX/Mathematics
- Справка по символам: https://en.wikibooks.org/wiki/LaTeX/Special_Characters
- Стили в LaTeX: https://en.wikibooks.org/wiki/LaTeX/Fonts#Math_mode

---

**Версия документации**: 1.0  
**Последнее обновление**: 2024
