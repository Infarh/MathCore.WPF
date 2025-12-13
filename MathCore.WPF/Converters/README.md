# Конвертеры значений WPF

Библиотека содержит более 100 конвертеров значений (IValueConverter) для использования в WPF-приложениях. Конвертеры позволяют преобразовывать значения при привязке данных в XAML.

## Содержание

- [Базовые классы](#базовые-классы)
- [Математические конвертеры](#математические-конвертеры)
- [Арифметические конвертеры](#арифметические-конвертеры)
- [Конвертеры сравнения](#конвертеры-сравнения)
- [Логические конвертеры](#логические-конвертеры)
- [Конвертеры диапазонов](#конвертеры-диапазонов)
- [Конвертеры преобразования типов](#конвертеры-преобразования-типов)
- [Конвертеры коллекций](#конвертеры-коллекций)
- [Конвертеры строк](#конвертеры-строк)
- [Конвертеры рефлексии](#конвертеры-рефлексии)
- [Конвертеры файловой системы](#конвертеры-файловой-системы)

## Базовые классы

### ValueConverter
Абстрактный базовый класс для всех конвертеров. Реализует интерфейс `IValueConverter` и `MarkupExtension`.

### DoubleValueConverter
Базовый класс для конвертеров, работающих с числовыми значениями типа `double`.

**Свойства**:
- `Min` — минимальное значение результата
- `Max` — максимальное значение результата

### SimpleDoubleValueConverter
Упрощенный конвертер для простых математических операций с double.

### MultiValueValueConverter
Базовый класс для мультиконвертеров (IMultiValueConverter).

## Математические конвертеры

### Abs
Вычисляет модуль (абсолютное значение) числа.

**Формула**: `result = |value|`

**Обратное преобразование**: ❌ Не поддерживается (неоднозначно)

**Пример использования**:
```xml
<TextBlock Text="{Binding Temperature, Converter={converters:Abs}}" />
```

### Sin
Вычисляет синус угла с возможностью масштабирования и смещения.

**Формула**: `result = K * sin(W * value) + B`

**Свойства**:
- `K` — коэффициент масштабирования амплитуды (по умолчанию 1)
- `B` — смещение результата (по умолчанию 0)
- `W` — угловая частота (по умолчанию 2π)

**Обратное преобразование**: ❌ Не поддерживается

**Пример**:
```xml
<Canvas.Top>
    <Binding Path="Index">
        <Binding.Converter>
            <converters:Sin K="50" B="100" W="0.1"/>
        </Binding.Converter>
    </Binding>
</Canvas.Top>
```

### Cos
Вычисляет косинус угла с возможностью масштабирования и смещения.

**Формула**: `result = K * cos(W * value) + B`

**Свойства**: аналогичны Sin

**Обратное преобразование**: ❌ Не поддерживается

### Tan
Вычисляет тангенс угла.

**Формула**: `result = K * tan(W * value) + B`

**Обратное преобразование**: ❌ Не поддерживается

### Ctg
Вычисляет котангенс угла.

**Формула**: `result = K / tan(W * value) + B`

**Обратное преобразование**: ❌ Не поддерживается

### Sign
Определяет знак числа.

**Формула**: `result = sign(W * value) * K + B`

**Возвращает**: -1, 0 или 1

**Обратное преобразование**: ❌ Не поддерживается

### Round
Округляет число до заданного количества десятичных разрядов.

**Свойства**:
- `Digits` — количество десятичных разрядов (по умолчанию 0)
- `Rounding` — режим округления промежуточных значений
- `K` — коэффициент масштабирования перед округлением

**Обратное преобразование**: ❌ Не поддерживается

**Пример**:
```xml
<TextBlock Text="{Binding Price, Converter={converters:Round Digits=2}}" />
```

### RoundAdaptive
Адаптивное округление с автоматическим выбором разрядности.

### Trunc / Truncate
Усекает дробную часть числа.

**Обратное преобразование**: ❌ Не поддерживается

### Inverse
Вычисляет обратное значение.

**Формула**: `result = 1/value`

**Обратное преобразование**: ✅ Поддерживается (также 1/value)

### Mod
Вычисляет остаток от деления.

**Формула**: `result = value % M`

**Свойство**: `M` — делитель

### dB
Преобразует амплитуду в децибелы.

**Формула**: `result = 20 * log10(value)`

### ExConverter
Вычисляет экспоненту.

**Формула**: `result = e^value`

### ExpConverter (Expr)
Конвертер вычисления математических выражений.

## Арифметические конвертеры

### Addition
Прибавляет константу к значению.

**Формула**: `result = value + P`

**Свойство**: `P` — прибавляемое значение

**Обратное преобразование**: ✅ Поддерживается (`value - P`)

**Пример**:
```xml
<TextBlock Width="{Binding ActualWidth, Converter={converters:Addition P=10}}" />
```

### Subtraction
Вычитает константу из значения.

**Формула**: `result = value - P`

**Обратное преобразование**: ✅ Поддерживается (`value + P`)

### Multiply
Умножает значение на константу.

**Формула**: `result = value * K`

**Обратное преобразование**: ✅ Поддерживается (`value / K`)

**Пример**:
```xml
<Rectangle Width="{Binding Height, Converter={converters:Multiply K=1.5}}" />
```

### Divide
Делит значение на константу.

**Формула**: `result = value / K`

**Обратное преобразование**: ✅ Поддерживается (`value * K`)

### Linear
Линейное преобразование.

**Формула**: `result = K * value + B`

**Свойства**:
- `K` — линейный коэффициент (тангенс угла наклона)
- `B` — аддитивное смещение
- `Inverted` — инвертировать преобразование

**Обратное преобразование**: ✅ Поддерживается (`(value - B) / K`)

**Пример**:
```xml
<TextBlock Opacity="{Binding Progress, Converter={converters:Linear K=0.01 B=0}}" />
```

### Arithmetic
Универсальный арифметический конвертер с поддержкой основных математических операций.

## Мультиконвертеры арифметики

### AdditionMulti
Суммирует несколько значений.

**Формула**: `result = value1 + value2 + ... + valueN`

**Пример**:
```xml
<MultiBinding Converter="{converters:AdditionMulti}">
    <Binding Path="Width"/>
    <Binding Path="Margin"/>
</MultiBinding>
```

### SubtractionMulti
Последовательно вычитает значения.

**Формула**: `result = value1 - value2 - ... - valueN`

### MultiplyMany
Перемножает несколько значений.

**Формула**: `result = value1 * value2 * ... * valueN`

### DivideMulti
Последовательно делит значения с обработкой деления на ноль.

**Формула**: `result = value1 / value2 / ... / valueN`

### AverageMulti
Вычисляет среднее арифметическое нескольких значений.

## Конвертеры сравнения

### GreaterThan
Проверяет, что значение больше заданного порога.

**Возвращает**: `bool`

**Свойство**: `Threshold` — пороговое значение

**Пример**:
```xml
<Button IsEnabled="{Binding Count, Converter={converters:GreaterThan Threshold=0}}" />
```

### GreaterThanOrEqual
Проверяет, что значение больше или равно порогу.

### LessThan
Проверяет, что значение меньше заданного порога.

### LessThanOrEqual
Проверяет, что значение меньше или равно порогу.

### GreaterThanMulti
Проверяет, что первое значение больше всех остальных.

### LessOrEqualThanMulti
Проверяет, что первое значение меньше или равно остальным.

## Логические конвертеры

### And
Логическое И (AND).

**Пример**:
```xml
<Button Visibility="{Binding IsValid, Converter={converters:And}}" />
```

### Or
Логическое ИЛИ (OR).

### Not
Логическое отрицание (NOT).

**Пример**:
```xml
<TextBlock Visibility="{Binding IsHidden, Converter={converters:Not}}" />
```

### Bool2Visibility
Преобразует булевое значение в Visibility.

**Возвращает**: `Visible` для `true`, `Collapsed` для `false`

**Пример**:
```xml
<Grid Visibility="{Binding IsVisible, Converter={converters:Bool2Visibility}}" />
```

## Конвертеры диапазонов

### InRange
Проверяет, находится ли значение в заданном диапазоне.

**Свойства**:
- `Min` — минимальное значение диапазона
- `Max` — максимальное значение диапазона
- `MinInclude` — включать ли минимум в диапазон
- `MaxInclude` — включать ли максимум в диапазон

**Возвращает**: `bool?`

**Пример**:
```xml
<Border Background="Green">
    <Border.Visibility>
        <Binding Path="Temperature">
            <Binding.Converter>
                <converters:InRange Min="18" Max="25"/>
            </Binding.Converter>
        </Binding>
    </Border.Visibility>
</Border>
```

### OutRange
Проверяет, находится ли значение вне заданного диапазона.

### Range
Ограничивает значение заданным диапазоном (clamp).

**Формула**: `result = max(Min, min(Max, value))`

### InIntervalValue
Проверяет нахождение значения в заданном интервале.

## Конвертеры преобразования типов

### ToString
Преобразует значение в строку.

**Пример**:
```xml
<TextBlock Text="{Binding Value, Converter={converters:ToString}}" />
```

### GetType
Получает тип объекта.

**Возвращает**: `Type`

### IsNull
Проверяет значение на null.

**Свойство**: `Inverted` — инвертировать результат

**Пример**:
```xml
<TextBlock Visibility="{Binding Data, Converter={converters:IsNull Inverted=True}}" />
```

### IsNaN
Проверяет значение на NaN.

**Возвращает**: `bool`

### IsPositive
Проверяет, что значение положительное.

### IsNegative
Проверяет, что значение отрицательное.

### DefaultIfNaN
Замена NaN на значение по умолчанию.

### NANtoVisibility / Null2Visibility
Преобразование NaN или null в Visibility.

### BoolToBrushConverter
Преобразование булевого значения в кисть (Brush).

### ColorBrushConverter
Преобразование цвета (Color) в кисть (Brush).

## Конвертеры коллекций

### ArrayElement
Получение элемента массива по индексу.

**Параметр**: индекс элемента

### FirstItemConverter
Получение первого элемента из коллекции.

### LastItemConverter
Получение последнего элемента из коллекции.

### FirstLastItemConverter
Получение первого или последнего элемента из коллекции.

### AggregateArray
Агрегирование массива с применением заданной операции.

### ArrayToStringConverter
Преобразование массива в строку.

### MultiValuesToEnumerable
Преобразование нескольких значений в перечисляемую коллекцию.

### MultiValuesToCompositeCollection
Преобразование нескольких значений в CompositeCollection.

### SingleValue
Возврат единственного фиксированного значения.

## Конвертеры строк

Расположены в подкаталоге `StringConverters/`.

### ToUpper
Преобразование строки к верхнему регистру.

**Пример**:
```xml
<TextBlock Text="{Binding Name, Converter={converters:ToUpper}}" />
```

### ToLower
Преобразование строки к нижнему регистру.

### JoinStringConverter
Объединение строк с разделителем.

**Параметр**: разделитель (по умолчанию пробел)

### DataLengthString
Преобразование размера данных в строковое представление (байты, КБ, МБ, ГБ и т.д.).

## Конвертеры рефлексии

Расположены в подкаталоге `Reflection/`.

Эти конвертеры извлекают информацию из атрибутов сборки (Assembly).

### AssemblyTitle
Получение заголовка из атрибутов сборки.

### AssemblyVersion
Получение версии из атрибутов сборки.

### AssemblyFileVersion
Получение версии файла из атрибутов сборки.

### AssemblyProduct
Получение названия продукта из атрибутов сборки.

### AssemblyCompany
Получение названия компании из атрибутов сборки.

### AssemblyCopyright
Получение информации об авторских правах из атрибутов сборки.

### AssemblyDescription
Получение описания из атрибутов сборки.

### AssemblyConfiguration
Получение конфигурации из атрибутов сборки.

### AssemblyTrademark
Получение торговой марки из атрибутов сборки.

### AssemblyTime
Получение времени сборки из атрибутов.

### GetTypeAssembly
Получение сборки (Assembly) из типа объекта.

**Пример**:
```xml
<TextBlock Text="{Binding Source={x:Type local:MainWindow}, Converter={converters:AssemblyVersion}}" />
```

## Конвертеры файловой системы

Расположены в подкаталоге `IO/`.

### FilePathToName
Извлечение имени файла из полного пути.

**Пример**:
```xml
<TextBlock Text="{Binding FilePath, Converter={converters:FilePathToName}}" />
```

### StringToFileInfo
Преобразование строки пути в объект FileInfo.

## Специальные конвертеры

### TemperatureC2F
Преобразование температуры из Цельсия в Фаренгейт.

**Формула**: `F = C * 9/5 + 32`

**Обратное преобразование**: ✅ Поддерживается

### TemperatureF2C
Преобразование температуры из Фаренгейта в Цельсий.

**Формула**: `C = (F - 32) * 5/9`

**Обратное преобразование**: ✅ Поддерживается

### SecondsToTimeSpan
Преобразование секунд в TimeSpan.

### TimeDifferential
Вычисление разницы во времени.

### Mapper
Отображение (маппинг) значений из одного диапазона в другой.

**Свойства**:
- `MinValue` — минимальное входное значение
- `MaxValue` — максимальное входное значение
- `MinScale` — минимальное выходное значение
- `MaxScale` — максимальное выходное значение

**Обратное преобразование**: ✅ Поддерживается

**Пример**:
```xml
<Slider Value="{Binding Volume}"/>
<TextBlock>
    <TextBlock.Text>
        <Binding Path="Volume">
            <Binding.Converter>
                <converters:Mapper MinValue="0" MaxValue="100" MinScale="-60" MaxScale="0"/>
            </Binding.Converter>
        </Binding>
    </TextBlock.Text>
    <TextBlock.Text> dB</TextBlock.Text>
</TextBlock>
```

### Interpolation
Линейная интерполяция значений.

### CSplineInterp
Кубическая сплайн-интерполяция.

### SwitchConverter / SwitchConverter2
Выбор значения по ключу (switch-case логика).

### Combine / CombineMulti
Объединение значений.

### Composite
Композитный конвертер для последовательного применения нескольких преобразований.

### Lambda / LambdaConverter / LambdaMultiConverter
Конвертеры с настраиваемой lambda-функцией преобразования.

### Custom / CustomMulti
Пользовательские конвертеры с настраиваемой логикой преобразования.

### AsyncConverter
Асинхронный конвертер для выполнения преобразований в фоновом потоке.

### Points2PathGeometry
Преобразование коллекции точек в PathGeometry.

### ValuesToPoint
Создание точки (Point) из двух значений.

### ArraysToPoints
Преобразование массивов в коллекцию точек.

### EnumEqual
Проверка равенства значения перечисления.

### Deviation
Вычисление отклонения от заданного значения.

### SignValue
Выбор значения в зависимости от знака числа.

### MaxValue / MinValue
Получение максимального/минимального значения.

### Average
Вычисление среднего значения.

## Общие замечания

### Обратное преобразование
Многие математические конвертеры не поддерживают обратное преобразование (ConvertBack), так как оно математически неоднозначно или невозможно. В таких случаях вызов ConvertBack бросает `NotSupportedException`.

### Использование параметров
Большинство конвертеров поддерживают передачу параметров через свойства или через параметр конвертера в XAML.

### Производительность
Для сложных вычислений рекомендуется использовать AsyncConverter, который выполняет преобразования в фоновом потоке.

### Граничные значения
Конвертеры корректно обрабатывают специальные значения: `NaN`, `PositiveInfinity`, `NegativeInfinity`.

## Примеры комплексного использования

### Пример 1: Прогресс-бар с процентами
```xml
<StackPanel>
    <ProgressBar Value="{Binding Progress}" Maximum="100"/>
    <TextBlock>
        <TextBlock.Text>
            <MultiBinding StringFormat="{}{0:F1}%">
                <Binding Path="Progress"/>
            </MultiBinding>
        </TextBlock.Text>
    </TextBlock>
</StackPanel>
```

### Пример 2: Масштабирование с ограничениями
```xml
<Image>
    <Image.Width>
        <Binding Path="BaseWidth">
            <Binding.Converter>
                <converters:Multiply K="1.5">
                    <converters:Multiply.Min>100</converters:Multiply.Min>
                    <converters:Multiply.Max>500</converters:Multiply.Max>
                </converters:Multiply>
            </Binding.Converter>
        </Binding>
    </Image.Width>
</Image>
```

### Пример 3: Условное отображение
```xml
<Border Background="Red" Visibility="{Binding Temperature, Converter={converters:OutRange Min=15 Max=25}}">
    <TextBlock Text="Температура вне нормы!" />
</Border>
```

## Расширение функциональности

Для создания собственных конвертеров рекомендуется наследоваться от базовых классов:
- `ValueConverter` — для общих конвертеров
- `DoubleValueConverter` — для числовых конвертеров
- `MultiValueValueConverter` — для мультиконвертеров

## Лицензия

Конвертеры являются частью библиотеки MathCore.WPF, распространяемой под лицензией MIT.

## Авторы

- Shmachilin P.A. (shmachilin@gmail.com)

## Ссылки

- [GitHub Repository](https://github.com/infarh/mathcore.wpf)
- [Документация WPF](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
- [IValueConverter](https://docs.microsoft.com/en-us/dotnet/api/system.windows.data.ivalueconverter)
