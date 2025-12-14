---

# Converters to refactoring — завершение

Дата: 2025-12-14

Статус: Добавление class‑level XML‑комментариев в конвертеры завершено

Кратко: на этапе добавлены XML `<summary>` для публичных классов в каталоге `MathCore.WPF/Converters` и подкаталогах; соответствующие пункты о необходимости добавления комментариев удалены из списков задач

Изменённые файлы (вторая половина):
- MathCore.WPF/Converters/JoinStringConverter.cs
- MathCore.WPF/Converters/LastItemConverter.cs
- MathCore.WPF/Converters/LessOrEqualThanMulti.cs
- MathCore.WPF/Converters/LessThan.cs
- MathCore.WPF/Converters/LessThanMulti.cs
- MathCore.WPF/Converters/LessThanOrEqual.cs
- MathCore.WPF/Converters/Linear.cs
- MathCore.WPF/Converters/Lambda.cs
- MathCore.WPF/Converters/LambdaConverter.cs
- MathCore.WPF/Converters/LambdaMulti.cs
- MathCore.WPF/Converters/Mapper.cs
- MathCore.WPF/Converters/MaxValue.cs
- MathCore.WPF/Converters/MinValue.cs
- MathCore.WPF/Converters/Mod.cs
- MathCore.WPF/Converters/Multiply.cs
- MathCore.WPF/Converters/MultiplyMany.cs
- MathCore.WPF/Converters/MultiValuesToCompositeCollection.cs
- MathCore.WPF/Converters/MultiValuesToEnumerable.cs
- MathCore.WPF/Converters/NANtoVisibility.cs
- MathCore.WPF/Converters/Null2Visibility.cs
- MathCore.WPF/Converters/Not.cs
- MathCore.WPF/Converters/OutRange.cs
- MathCore.WPF/Converters/Or.cs
- MathCore.WPF/Converters/Points2PathGeometry.cs
- MathCore.WPF/Converters/Range.cs
- MathCore.WPF/Converters/Reflection/AssemblyCompany.cs
- MathCore.WPF/Converters/Reflection/AssemblyConfiguration.cs
- MathCore.WPF/Converters/Reflection/AssemblyConverter.cs
- MathCore.WPF/Converters/Reflection/AssemblyCopyright.cs
- MathCore.WPF/Converters/Reflection/AssemblyDescription.cs
- MathCore.WPF/Converters/Reflection/AssemblyFileVersion.cs
- MathCore.WPF/Converters/Reflection/AssemblyProduct.cs
- MathCore.WPF/Converters/Reflection/AssemblyTime.cs
- MathCore.WPF/Converters/Reflection/AssemblyTitle.cs
- MathCore.WPF/Converters/Reflection/AssemblyTrademark.cs
- MathCore.WPF/Converters/Reflection/AssemblyVersion.cs
- MathCore.WPF/Converters/Round.cs
- MathCore.WPF/Converters/RoundAdaptive.cs
- MathCore.WPF/Converters/Sign.cs
- MathCore.WPF/Converters/SignValue.cs
- MathCore.WPF/Converters/Sin.cs
- MathCore.WPF/Converters/SecondsToTimeSpan.cs
- MathCore.WPF/Converters/SingleValue.cs
- MathCore.WPF/Converters/SwitchConverter.cs
- MathCore.WPF/Converters/SwitchConverter2.cs
- MathCore.WPF/Converters/StringConverters/ToLower.cs
- MathCore.WPF/Converters/StringConverters/ToUpper.cs
- MathCore.WPF/Converters/Subtraction.cs
- MathCore.WPF/Converters/SubtractionMulti.cs
- MathCore.WPF/Converters/TemperatureC2F.cs
- MathCore.WPF/Converters/TemperatureF2C.cs
- MathCore.WPF/Converters/Tan.cs
- MathCore.WPF/Converters/TimeDifferential.cs
- MathCore.WPF/Converters/ToString.cs
- MathCore.WPF/Converters/Truncate.cs
- MathCore.WPF/Converters/Trunc.cs
- MathCore.WPF/Converters/ValuesToPoint.cs

---

## Найденные проблемы (краткие по файлам)

- MathCore.WPF/Converters/JoinStringConverter.cs
  - `ConvertBack` реализован некорректно в исходной версии; заменить на безопасный `Split(...).Cast<object>().ToArray()` и обработать null

- MathCore.WPF/Converters/LastItemConverter.cs
  - Поведение корректно; добавить XML‑документацию и тесты

- MathCore.WPF/Converters/LessOrEqualThanMulti.cs
  - Добавить XML‑документацию и тесты

- MathCore.WPF/Converters/LessThan.cs
  - Проверить NaN проверку и документировать поведение

- MathCore.WPF/Converters/LessThanMulti.cs
  - Добавить XML‑документацию и тесты

- MathCore.WPF/Converters/LessThanOrEqual.cs
  - Исправить проверку NaN и добавить документацию

- MathCore.WPF/Converters/Linear.cs
  - Документировать поведение при K==0 (деление в ConvertBack)

- MathCore.WPF/Converters/Lambda.cs
  - Проверить приведения типов в Convert/ConvertBack и добавить проверки `is TValue`/`is TResult`

- MathCore.WPF/Converters/LambdaConverter.cs
  - Document ConvertBack throws NotSupportedException when From is null

- MathCore.WPF/Converters/LambdaMulti.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/Mapper.cs
  - При пересчёте коэффициента `_k` учтено деление на ноль — рекомендовано `_k = 0` и ConvertBack возвращает NaN

- MathCore.WPF/Converters/MaxValue.cs
  - `vv.Max()` может бросить исключение для несравнимых типов; документировать и защищать

- MathCore.WPF/Converters/MinValue.cs
  - Аналогично MaxValue: защитить от несравнимых типов

- MathCore.WPF/Converters/Mod.cs
  - Проверить поведение при M==0 и NaN; использовать `double.IsNaN`

- MathCore.WPF/Converters/Multiply.cs
  - Корректно; добавить тесты

- MathCore.WPF/Converters/MultiplyMany.cs
  - Унифицировать поведение при null и null-элементах; использовать TryConvert

- MathCore.WPF/Converters/MultiValuesToCompositeCollection.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/MultiValuesToEnumerable.cs
  - ConvertBack требует проверок на tt и длину коллекции; избежать возможных NRE

- MathCore.WPF/Converters/NANtoVisibility.cs
  - Проверять тип входа перед броском приведения; унифицировать возврат при null

- MathCore.WPF/Converters/Null2Visibility.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/Not.cs
  - Обработку null/не-bool привести к явной проверке `is bool b ? !b : Binding.DoNothing`

- MathCore.WPF/Converters/OutRange.cs
  - Проверить использование interval и IsNaN; добавить документацию

- MathCore.WPF/Converters/Or.cs
  - Не использовать `Cast<bool>()`; безопасно перебирать и проверять типы

- MathCore.WPF/Converters/Points2PathGeometry.cs
  - Проверить совместимость pattern-matching с целевыми TFM; документировать возврат null

- MathCore.WPF/Converters/Range.cs
  - Проверить primary constructor синтаксис и интерфейс Interval

- MathCore.WPF/Converters/Reflection/AssemblyCompany.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/Reflection/AssemblyConfiguration.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/Reflection/AssemblyConverter.cs
  - В Convert использовать безопасную проверку типа Assembly

- MathCore.WPF/Converters/Reflection/AssemblyCopyright.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/Reflection/AssemblyDescription.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/Reflection/AssemblyFileVersion.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/Reflection/AssemblyProduct.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/Reflection/AssemblyTime.cs
  - Проверить `Assembly.Location` на пустую строку и документировать поведение

- MathCore.WPF/Converters/Reflection/AssemblyTitle.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/Reflection/AssemblyTrademark.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/Reflection/AssemblyVersion.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/Round.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/RoundAdaptive.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/Sign.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/SignValue.cs
  - Документировать ожидаемые диапазоны и поведение при NaN

- MathCore.WPF/Converters/Sin.cs
  - Реализация корректна; добавить XML‑документацию

- MathCore.WPF/Converters/SecondsToTimeSpan.cs
  - Добавить XML‑документацию и тесты

- MathCore.WPF/Converters/SingleValue.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/SwitchConverter.cs
  - Добавить XML‑документацию и проверить ConvertBack

- MathCore.WPF/Converters/SwitchConverter2.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/StringConverters/ToLower.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/StringConverters/ToUpper.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/Subtraction.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/SubtractionMulti.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/TemperatureC2F.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/TemperatureF2C.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/Tan.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/TimeDifferential.cs
  - Добавить XML‑документацию и тесты

- MathCore.WPF/Converters/ToString.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/Truncate.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/Trunc.cs
  - Добавить XML‑документацию

- MathCore.WPF/Converters/ValuesToPoint.cs
  - Добавить XML‑документацию

---

# Примечание
Задача по добавлению class‑level XML‑комментариев в конвертеры завершена и удалена из списка активных задач
