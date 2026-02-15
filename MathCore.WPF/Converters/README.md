# Конвертеры MathCore.WPF

Каталог содержит полный набор WPF конвертеров для преобразования значений в привязках данных. Все конвертеры наследуют от базовых классов и поддерживают использование в XAML как расширения разметки.

## Архитектура

### Базовые классы

#### `ValueConverter`
Абстрактный базовый класс для всех конвертеров. Реализует интерфейсы `IValueConverter` и `MarkupExtension`, позволяя использовать конвертеры непосредственно в XAML.

- **Методы**: `Convert()`, `ConvertBack()`
- **Возвращаемое значение из XAML**: сам конвертер через `ProvideValue()`

#### `DoubleValueConverter`
Специализированный класс для работы с числовыми значениями типа `double`. Упрощает реализацию конвертеров, работающих с вещественными числами.

#### `SimpleDoubleValueConverter`
Вспомогательный класс для простых арифметических операций с указанием функций преобразования вперёд и обратно.

```csharp
public class Addition(double P) : SimpleDoubleValueConverter(P, 
    (v, p) => v + p,      // Convert: v + P
    (r, p) => r - p)      // ConvertBack: r - P
{
    public Addition() : this(0) { }
}
```

#### `MultiValueValueConverter`
Базовый класс для конвертеров, работающих с несколькими значениями одновременно (реализуют `IMultiValueConverter`).

#### `MultiDoubleValueValueConverter`
Специализированный класс для работы с несколькими числовыми значениями.

#### `DoubleToBool`
Базовый класс для конвертеров, преобразующих числовые значения в логические.

---

## Арифметические конвертеры

### `Abs`
**Назначение**: Возвращает абсолютное значение (модуль) числа

**XAML использование**:
```xaml
{Binding Value, Converter={Abs}}
```

**Пример**: `-5` → `5`, `3.14` → `3.14`

---

### `Addition`
**Назначение**: Прибавляет к значению указанное число

**Параметры**: `P` (double, по умолчанию 0) — значение для прибавления

**XAML использование**:
```xaml
{Binding Value, Converter={Addition 5}}
{Binding Value, Converter={Addition P=5}}
```

**Пример**: `10 + 5 = 15`

---

### `Subtraction`
**Назначение**: Вычитает заданное значение из входного

**Параметры**: `Parameter` (double) — значение для вычитания

**XAML использование**:
```xaml
{Binding Value, Converter={Subtraction 3}}
{Binding Value, Converter={Subtraction Parameter=3}}
```

**Пример**: `10 - 3 = 7`

---

### `Multiply`
**Назначение**: Умножает значение на указанный параметр

**Параметры**: `Parameter` (double) — множитель

**XAML использование**:
```xaml
{Binding Value, Converter={Multiply 2}}
{Binding Value, Converter={Multiply Parameter=2}}
```

**Пример**: `5 * 2 = 10`

---

### `Divide`
**Назначение**: Выполняет деление значения на указанное число

**Параметры**: `Parameter` (double) — делитель

**XAML использование**:
```xaml
{Binding Value, Converter={Divide 10}}
{Binding Value, Converter={Divide Parameter=10}}
```

**Пример**: `20 / 10 = 2`

**Особенности**: При делении на ноль возвращает `NaN` или `Infinity`

---

### `Mod`
**Назначение**: Вычисляет остаток от деления (модуль по числу)

**Параметры**: `M` (double) — делитель

**XAML использование**:
```xaml
{Binding Value, Converter={Mod 5}}
{Binding Value, Converter={Mod M=5}}
```

**Пример**: `13 mod 5 = 3`

---

### `Linear`
**Назначение**: Выполняет линейное преобразование по формуле `f(x) = K*x + B`

**Параметры**: 
- `K` (double) — коэффициент масштаба
- `B` (double) — смещение

**XAML использование**:
```xaml
{Binding Value, Converter={Linear 3, 5}}
{Binding Value, Converter={Linear K=3, B=5}}
{Binding Value, Converter={Linear K=2}}
```

**Пример**: `f(2) = 3*2 + 5 = 11`

**Особенности**: Поддерживает обратное преобразование; при `K=0` в `ConvertBack` возвращает `NaN`

---

### `Mapper`
**Назначение**: Преобразует физическое значение в экранное значение (масштабирование диапазонов)

**Параметры**:
- `MinScale`, `MaxScale` — диапазон экранных значений
- `MinValue`, `MaxValue` — диапазон физических значений

**XAML использование**:
```xaml
{Binding Value, Converter={Mapper MinScale=-200, MaxScale=400, MinValue=-5, MaxValue=5}}
```

**Пример**: Значение `-5` → `-200`, значение `5` → `400`

**Особенности**: Линейное масштабирование между диапазонами

---

### `MapperConverter`
**Назначение**: Freezable-версия конвертера Mapper для использования в XAML с наследованием DataContext

**Параметры**:
- `MinScale`, `MaxScale` — диапазон экранных значений (DependencyProperty)
- `MinValue`, `MaxValue` — диапазон физических значений (DependencyProperty)

**XAML использование в ресурсах с привязками**:
```xaml
<Window.Resources>
  <local:MapperConverter x:Key="TempToAngleMapper" 
                         MinValue="0" MaxValue="100" 
                         MinScale="0" MaxScale="360" />
</Window.Resources>

<RotateTransform Angle="{Binding Temperature, Converter={StaticResource TempToAngleMapper}}" />
```

**XAML использование с динамическими привязками**:
```xaml
<local:MapperConverter MinValue="{Binding MinValue}" 
                       MaxValue="{Binding MaxValue}"
                       MinScale="0" MaxScale="100" />
```

**Пример**: Значение `50` из диапазона `[0, 100]` → `180` в диапазон `[0, 360]`

**Особенности**: 
- Наследует от `Freezable`, что позволяет получать `DataContext` из дерева XAML
- Все свойства реализованы как `DependencyProperty` для полной поддержки привязок
- Поддерживает обратное преобразование (`ConvertBack`)
- Пересчитывает коэффициент масштабирования автоматически при изменении любого параметра диапазона

---

### `MapperF`
**Назначение**: Расширение разметки для быстрого создания MapperConverter в привязках

**Параметры**:
- `MinValue`, `MaxValue` — диапазон физических значений
- `MinScale`, `MaxScale` — диапазон экранных значений

**XAML использование в привязке (простой синтаксис)**:
```xaml
<RotateTransform Angle="{Binding Temperature, 
    Converter={local:MapperF MinValue=0, MaxValue=100, MinScale=0, MaxScale=360}}" />
```

**Пример**: Температура `50°C` → поворот на `180°`

**Особенности**: 
- Упрощённый синтаксис для статических конфигураций
- Создаёт новый независимый экземпляр `MapperConverter` при каждом вызове
- Используйте `MapperConverter` напрямую в ресурсах, если нужны динамические привязки параметров

---

### `Inverse`
**Назначение**: Вычисляет значение по формуле `f(x) = Parameter / x`

**Параметры**: `Parameter` (double, по умолчанию 1)

**XAML использование**:
```xaml
{Binding Value, Converter={Inverse}}
{Binding Value, Converter={Inverse 5}}
```

**Пример**: `f(2) = 1/2 = 0.5`, `f(4) = 5/4 = 1.25`

---

### `Round`
**Назначение**: Округляет значение до указанного количества знаков после запятой

**Параметры**: `Digits` (int) — количество знаков

**XAML использование**:
```xaml
{Binding Value, Converter={Round}}
{Binding Value, Converter={Round 5}}
```

**Пример**: `3.14159` → `3.14` (при `Digits=2`)

---

### `RoundAdaptive`
**Назначение**: Округляет значение в адаптивном режиме (похоже на `Round`, но адаптирует к контексту)

**XAML использование**:
```xaml
{Binding Value, Converter={RoundAdaptive}}
```

---

### `Truncate` / `Trunc`
**Назначение**: Отбрасывает дробную часть вещественного числа

**XAML использование**:
```xaml
{Binding Value, Converter={Truncate}}
{Binding Value, Converter={Trunc}}
```

**Пример**: `3.99` → `3.0`

---

## Логические операции

### `And`
**Назначение**: Логическое И для нескольких значений

**XAML использование**:
```xaml
<MultiBinding Converter={And}>
    <Binding Path="IsEnabled" />
    <Binding Path="IsValid" />
</MultiBinding>
```

**Поведение**: Возвращает `true` только если все входные значения логически истинны

---

### `Or`
**Назначение**: Логическое ИЛИ для нескольких значений

**XAML использование**:
```xaml
<MultiBinding Converter={Or}>
    <Binding Path="IsError" />
    <Binding Path="IsWarning" />
</MultiBinding>
```

**Поведение**: Возвращает `true` если хотя бы одно значение логически истинно

---

### `Not`
**Назначение**: Логическое отрицание (инверсия) логического значения

**XAML использование**:
```xaml
{Binding IsEnabled, Converter={Not}}
```

**Пример**: `true` → `false`, `false` → `true`

---

## Сравнение значений

### `GreaterThan`
**Назначение**: Проверяет, больше ли значение указанного параметра

**Параметры**: `Value` (double) — пороговое значение

**XAML использование**:
```xaml
{Binding Value, Converter={GreaterThan 3.14}}
```

**Пример**: `5 > 3.14` → `true`

---

### `GreaterThanMulti`
**Назначение**: Проверяет, больше ли все значения указанного параметра

**XAML использование**:
```xaml
<MultiBinding Converter={GreaterThanMulti 10}>
    <Binding Path="Value1" />
    <Binding Path="Value2" />
</MultiBinding>
```

---

### `GreaterThanOrEqual`
**Nazначение**: Проверяет, больше или равно ли значение параметру

**Параметры**: `Value` (double) — пороговое значение

**XAML использование**:
```xaml
{Binding Value, Converter={GreaterThanOrEqual 5}}
```

---

### `GreaterOrEqualThanMulti`
**Назначение**: Проверяет, больше или равны ли все значения параметру

---

### `LessThan`
**Назначение**: Проверяет, меньше ли значение параметра

**Параметры**: `Value` (double) — пороговое значение

**XAML использование**:
```xaml
{Binding Value, Converter={LessThan 100}}
```

**Пример**: `50 < 100` → `true`

---

### `LessThanMulti`
**Назначение**: Проверяет, меньше ли все значения параметра

---

### `LessThanOrEqual`
**Назначение**: Проверяет, меньше или равно ли значение параметру

**Параметры**: `Value` (double) — пороговое значение

---

### `LessOrEqualThanMulti`
**Назначение**: Проверяет, меньше или равны ли все значения параметру

---

## Интервалы и диапазоны

### `InRange` / `InIntervalValue`
**Назначение**: Проверяет, находится ли значение в указанном диапазоне

**Параметры**:
- `Min` (double) — минимум диапазона
- `Max` (double) — максимум диапазона
- `MinInclude` (bool) — включать ли минимум
- `MaxInclude` (bool) — включать ли максимум

**XAML использование**:
```xaml
{Binding Value, Converter={InRange 5, 7}}
{Binding Value, Converter={InRange Min=5, Max=7, MinInclude=True, MaxInclude=True}}
{Binding Value, Converter={InRange 3}}
```

**Пример**: `6` в диапазоне `[5, 7]` → `true`

---

### `OutRange`
**Назначение**: Проверяет, находится ли значение ВНЕ указанного диапазона (антипод `InRange`)

**XAML использование**:
```xaml
{Binding Value, Converter={OutRange 5, 7}}
```

---

### `Range`
**Назначение**: Ограничивает значение указанным интервалом (зажим значения)

**Параметры**:
- `Min` (double) — минимум
- `Max` (double) — максимум

**XAML использование**:
```xaml
{Binding Value, Converter={Range 5, 7}}
{Binding Value, Converter={Range Min=5, Max=7}}
```

**Пример**: Значение `3` → `5`, значение `10` → `7`, значение `6` → `6`

---

## Проверки значений

### `IsNaN`
**Назначение**: Проверяет, является ли значение `NaN` (Not a Number)

**Параметры**: 
- `Inverted` (bool) — инвертировать результат

**XAML использование**:
```xaml
{Binding Value, Converter={IsNaN}}
{Binding Value, Converter={IsNaN Inverted=True}}
```

**Пример**: `double.NaN` → `true`, `5.0` → `false`

---

### `IsNull`
**Назначение**: Проверяет, является ли значение `null`

**Параметры**: `Inverted` (bool) — инвертировать результат

**XAML использование**:
```xaml
{Binding Value, Converter={IsNull}}
{Binding Value, Converter={IsNull Inverted=True}}
```

---

### `IsPositive`
**Назначение**: Проверяет, является ли значение положительным (> 0)

**XAML использование**:
```xaml
{Binding Value, Converter={IsPositive}}
```

---

### `IsNegative`
**Назначение**: Проверяет, является ли значение отрицательным (< 0)

**XAML использование**:
```xaml
{Binding Value, Converter={IsNegative}}
```

---

## Знак и абсолютное значение

### `Sign`
**Назначение**: Возвращает знак числа в виде `-1`, `0` или `+1`

**XAML использование**:
```xaml
{Binding Value, Converter={Sign}}
```

**Пример**: `-5` → `-1`, `0` → `0`, `5` → `1`

---

### `SignValue`
**Назначение**: Возвращает знак числа, но с учётом мёртвой зоны

**Параметры**:
- `Delta` (double) — размер мёртвой зоны
- `Inverse` (bool) — инвертировать результат

**XAML использование**:
```xaml
{Binding Value, Converter={SignValue Delta=5}}
```

**Пример**: При `Delta=5`, значение `-3` → `0`, `-10` → `-1`, `8` → `1`

---

## Видимость

### `Bool2Visibility`
**Назначение**: Преобразует логическое значение в видимость элемента

**Параметры**:
- `Inverted` (bool) — инвертировать результат
- `Collapsed` (bool) — использовать `Collapsed` вместо `Hidden`

**XAML использование**:
```xaml
<Button Content="Ok" Visibility="{Binding IsVisible, Converter={Bool2Visibility}}" />
{Binding IsVisible, Converter={Bool2Visibility Inverted=True}}
{Binding IsVisible, Converter={Bool2Visibility Collapsed=True}}
```

**Поведение**: 
- `true` → `Visible`
- `false` → `Hidden` (или `Collapsed` если указан флаг)

---

### `Null2Visibility`
**Назначение**: Преобразует `null` в видимость

**Параметры**:
- `Inverted` (bool) — инвертировать
- `Collapsed` (bool) — использовать `Collapsed`

**XAML использование**:
```xaml
{Binding Value, Converter={Null2Visibility}}
```

**Поведение**: 
- `null` → `Hidden`
- Другие значения → `Visible`

---

### `NANtoVisibility`
**Назначение**: Преобразует `NaN` в видимость

**Параметры**:
- `Inverted` (bool) — инвертировать
- `Collapsed` (bool) — использовать `Collapsed`

**XAML использование**:
```xaml
{Binding Value, Converter={NANtoVisibility}}
```

**Поведение**: 
- `NaN` → `Hidden`
- Нормальные значения → `Visible`

---

## Агрегирование и статистика

### `Addition` / `AdditionMulti`
**Назначение**: Суммирует несколько значений

**XAML использование**:
```xaml
<MultiBinding Converter={AdditionMulti}>
    <Binding Path="Value1" />
    <Binding Path="Value2" />
    <Binding Path="Value3" />
</MultiBinding>
```

---

### `Average` / `AverageMulti`
**Назначение**: Вычисляет среднее арифметическое

**Параметры**: `Length` (int) — размер скользящего окна для `Average`

**XAML использование**:
```xaml
{Binding Value, Converter={Average 5}}
<MultiBinding Converter={AverageMulti}>
    <Binding Path="Value1" />
    <Binding Path="Value2" />
</MultiBinding>
```

---

### `MinValue` / `MaxValue`
**Назначение**: Находит минимальное или максимальное значение

**XAML использование**:
```xaml
<MultiBinding Converter={MinValue}>
    <Binding Path="Value1" />
    <Binding Path="Value2" />
    <Binding Path="Value3" />
</MultiBinding>
```

---

### `Multiply` / `MultiplyMany`
**Назначение**: Умножает несколько значений

**XAML использование**:
```xaml
<MultiBinding Converter={MultiplyMany}>
    <Binding Path="Value1" />
    <Binding Path="Value2" />
</MultiBinding>
```

---

## Математические функции

### `Sin`
**Назначение**: Вычисляет синус значения (в радианах)

**XAML использование**:
```xaml
{Binding Angle, Converter={Sin}}
```

---

### `Cos`
**Назначение**: Вычисляет косинус значения (в радианах)

---

### `Tan`
**Назначение**: Вычисляет тангенс значения (в радианах)

---

### `Ctg`
**Назначение**: Вычисляет котангенс значения (в радианах)

---

### `dB`
**Назначение**: Преобразует значение в децибеллы или из децибеллов

**Параметры**:
- `ByPower` (bool) — использовать формулу для мощности
- `Invert` (bool) — обратное преобразование (из дБ в линейное)

**XAML использование**:
```xaml
{Binding Value, Converter={dB}}
{Binding Value, Converter={dB ByPower=True}}
{Binding Value, Converter={dB Invert=True}}
```

**Формулы**:
- Напряжение: `f(x) = 20*log10(x)`
- Мощность: `f(x) = 10*log10(x)`
- Обратное: `f(x) = 10^(x/20)` или `10^(x/10)`

---

## Интерполяция и аппроксимация

### `Interpolation`
**Назначение**: Выполняет полиномиальную интерполяцию по методу Лагранжа

**Параметры**: `Points` (string) — строка с точками интерполяции формата `x1,y1 x2,y2 x3,y3`

**XAML использование**:
```xaml
{Binding Value, Converter={Interpolation Points='1.2,3 3.2,7 12,1'}}
```

**Пример**: По трём точкам строится полином 2-й степени, затем вычисляется значение для входного X

---

### `CSplineInterp`
**Назначение**: Выполняет интерполяцию кубическим сплайном

**Параметры**: `Points` (string) — точки интерполяции

**XAML использование**:
```xaml
{Binding Value, Converter={CSplineInterp Points='1,3 2.5,7 7.9,12'}}
```

---

## Комбинирование конвертеров

### `Combine`
**Назначение**: Применяет два конвертера последовательно

**Параметры**:
- `First` (IValueConverter) — первый конвертер
- `Then` (IValueConverter) — второй конвертер

**XAML использование**:
```xaml
{Binding Value, Converter={Combine First={Addition 5} Then={Average 10}}}
{Binding Value, Converter={Combine {Addition 5}, {Average 10}}}
```

**Поток**: Значение → `First` → `Then` → Результат

---

### `CombineMulti`
**Назначение**: Комбинирует несколько конвертеров для работы с `IMultiValueConverter`

---

### `Composite`
**Назначение**: Композитный конвертер для сложных преобразований

---

### `Custom`
**Назначение**: Кастомный конвертер с использованием делегатов

**Параметры**: Функции `Convert` и `ConvertBack`

---

### `CustomMulti`
**Назначение**: Кастомный конвертер для множественных значений

---

## Преобразование типов

### `ToString`
**Назначение**: Преобразует значение в строку

**XAML использование**:
```xaml
{Binding Value, Converter={ToString}}
```

---

### `GetType`
**Назначение**: Получает тип значения

**XAML использование**:
```xaml
{Binding Value, Converter={GetType}}
```

---

### `EnumEqual`
**Назначение**: Сравнивает значение с перечислением

**XAML использование**:
```xaml
{Binding Status, Converter={EnumEqual {x:Static local:StatusEnum.Active}}}
```

---

## Строковые операции

### `ToUpper`
**Назначение**: Преобразует строку в верхний регистр

**XAML использование**:
```xaml
{Binding Text, Converter={ToUpper}}
```

---

### `ToLower`
**Назначение**: Преобразует строку в нижний регистр

**XAML использование**:
```xaml
{Binding Text, Converter={ToLower}}
```

---

### `ArrayToStringConverter`
**Назначение**: Преобразует массив в строку

**XAML использование**:
```xaml
{Binding Items, Converter={ArrayToStringConverter}}
```

---

### `JoinStringConverter`
**Назначение**: Объединяет значения в строку или разбивает строку на значения

**Параметры**: Разделитель (по умолчанию `,`)

---

## Сборка (Reflection)

### `AssemblyVersion`
**Назначение**: Получает версию сборки

**XAML использование**:
```xaml
{Binding Assembly, Converter={AssemblyVersion}}
```

---

### `AssemblyTitle`
**Назначение**: Получает заголовок сборки

---

### `AssemblyProduct`
**Назначение**: Получает наименование продукта

---

### `AssemblyCompany`
**Назначение**: Получает наименование компании

---

### `AssemblyDescription`
**Назначение**: Получает описание сборки

---

### `AssemblyCopyright`
**Назначение**: Получает информацию об авторских правах

---

### `AssemblyTrademark`
**Назначение**: Получает информацию о товарном знаке

---

### `AssemblyConfiguration`
**Назначение**: Получает конфигурацию сборки (Debug/Release)

---

### `AssemblyFileVersion`
**Назначение**: Получает версию файла

---

### `AssemblyTime`
**Назначение**: Получает время компиляции сборки

---

### `GetTypeAssembly`
**Назначение**: Получает сборку для типа

---

## Специализированные операции

### `Deviation`
**Назначение**: Вычисляет разность между текущим и предыдущим значением

**XAML использование**:
```xaml
{Binding Value, Converter={Deviation}}
```

**Пример**: Последовательность `1, 3, 5` → результаты `null, 2, 2`

---

### `TimeDifferential`
**Назначение**: Выполняет дифференцирование значения по времени

**Параметры**:
- `Parameter` (double) — задержка
- `IgnoreNaN` (bool) — игнорировать ли NaN

**XAML использование**:
```xaml
{Binding Value, Converter={TimeDifferential 15}}
{Binding Value, Converter={TimeDifferential Parameter=15, IgnoreNaN=False}}
```

---

### `DefaultIfNaN`
**Назначение**: Заменяет `NaN` значением по умолчанию

**XAML использование**:
```xaml
{Binding Value, Converter={DefaultIfNaN}}
```

---

### `DataLengthString`
**Назначение**: Преобразует размер данных в строку (Б, КБ, МБ, ГБ)

**XAML использование**:
```xaml
{Binding FileSize, Converter={DataLengthString}}
```

---

### `Temperature` конвертеры

#### `TemperatureC2F`
**Назначение**: Преобразует температуру из Цельсия в Фаренгейт

**Формула**: `f(x) = x / 1.8 - 32 / 1.8`

---

#### `TemperatureF2C`
**Назначение**: Преобразует температуру из Фаренгейта в Цельсий

**Формула**: `f(x) = 1.8 * x + 32`

---

### `SecondsToTimeSpan`
**Назначение**: Преобразует количество секунд в `TimeSpan`

**XAML использование**:
```xaml
{Binding Seconds, Converter={SecondsToTimeSpan}}
```

---

## Сложные операции

### `Lambda` / `LambdaConverter` / `LambdaMulti`
**Назначение**: Применяет лямбда-функцию для преобразования

**XAML использование**: Требует определения функции преобразования

---

### `AggregateArray`
**Назначение**: Агрегирует значения массива

---

### `ArrayElement`
**Назначение**: Получает элемент массива по индексу

**XAML использование**:
```xaml
{Binding Items, Converter={ArrayElement 0}}
```

---

### `FirstLastItemConverter`
**Назначение**: Получает первый или последний элемент коллекции

---

### `LastItemConverter`
**Назначение**: Получает последний элемент коллекции

---

### `SingleValue`
**Назначение**: Извлекает одиночное значение

---

### `ValuesToPoint`
**Назначение**: Преобразует два значения в точку (Point)

**XAML использование**:
```xaml
<MultiBinding Converter={ValuesToPoint}>
    <Binding Path="X" />
    <Binding Path="Y" />
</MultiBinding>
```

---

### `Points2PathGeometry`
**Назначение**: Преобразует последовательность точек в геометрический путь

---

### `ArraysToPoints`
**Назначение**: Преобразует массивы координат в массив точек

---

### `MultiValuesToEnumerable`
**Назначение**: Преобразует множественные значения в перечисление

---

### `MultiValuesToCompositeCollection`
**Назначение**: Преобразует значения в составную коллекцию

---

## Специальные конвертеры

### `AsyncConverter`
**Назначение**: Поддерживает асинхронные операции преобразования

---

### `ColorBrushConverter`
**Назначение**: Преобразует цвет в кисть и обратно

---

### `BoolToBrushConverter`
**Назначение**: Преобразует логическое значение в кисть

**XAML использование**:
```xaml
{Binding IsActive, Converter={BoolToBrushConverter}}
```

---

### `SwitchConverter` / `SwitchConverter2`
**Назначение**: Реализует логику переключателя для выбора значения

---

### `ExConverter`
**Назначение**: Расширенный конвертер с дополнительной функциональностью

---

### `ExpConverter`
**Назначение**: Вычисляет экспоненту

**XAML использование**:
```xaml
{Binding Value, Converter={ExpConverter}}
```

---

### `Arithmetic`
**Назначение**: Вычисляет арифметическое выражение

---

### `Mapper`
**Назначение**: Преобразует физическое значение в экранное значение (масштабирование диапазонов)

**Параметры**:
- `MinScale`, `MaxScale` — диапазон экранных значений
- `MinValue`, `MaxValue` — диапазон физических значений

**XAML использование**:
```xaml
{Binding Value, Converter={Mapper MinScale=-200, MaxScale=400, MinValue=-5, MaxValue=5}}
```

**Пример**: Значение `-5` → `-200`, значение `5` → `400`

**Особенности**: Линейное масштабирование между диапазонами

---

## Рекомендации по использованию

### Комбинирование конвертеров

```xaml
<!-- Сложить 5, затем проверить > 10 -->
<MultiBinding Converter={GreaterThan 10}
              ConverterParameter="{Binding ElementName=root, Path=DataContext}">
    <Binding Path="Value" Converter={Addition 5}" />
</MultiBinding>
```

### Обработка null значений

```xaml
<!-- Показать элемент только если значение не null и > 0 -->
<MultiBinding Converter={And}>
    <Binding Path="Value" Converter={IsNull Inverted=True}" />
    <Binding Path="Value" Converter={IsPositive}" />
</MultiBinding>
```

### Масштабирование для UI

```xaml
<!-- Преобразовать физическое значение в координаты экрана -->
<Canvas>
    <Rectangle 
        Canvas.Left="{Binding Coordinate, Converter={Mapper MinScale=0, MaxScale=500, MinValue=0, MaxValue=100}}"
        Canvas.Top="{Binding Height, Converter={Mapper MinScale=0, MaxScale=400, MinValue=0, MaxValue=100}}" />
</Canvas>
```

---

## Обработка ошибок

### NaN обработка

Конвертеры, работающие с вещественными числами, должны корректно обрабатывать `NaN`:

```csharp
if (double.IsNaN(value))
    return Binding.DoNothing;  // или другой результат
```

### Null значения

Используйте `Null2Visibility` для обработки null в UI:

```xaml
{Binding Value, Converter={Null2Visibility Inverted=True}}  <!-- видим если не null -->
```

---

## Заключение

MathCore.WPF предоставляет полный набор конвертеров для решения типичных задач преобразования данных в WPF приложениях. Все конвертеры:

- ✅ Поддерживают XAML как расширения разметки
- ✅ Типизированы и безопасны при использовании
- ✅ Поддерживают однонаправленное и двустороннее преобразование (где применимо)
- ✅ Документированы с примерами использования
- ✅ Оптимизированы для производства

Для получения дополнительной информации смотрите исходные файлы конвертеров в каталоге.
