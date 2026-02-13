# Правила валидации

В каталоге `ValidationRules` находятся правила валидации для WPF. Все правила используются в `ValidationRules` коллекции привязок.

## Общие параметры

Некоторые правила наследуются от базовых классов:

- `ValueValidation`: параметр `ErrorMessage` для сообщения об ошибке.
- `NullValueValidation`: параметры `AllowNull` и `NullReferenceMessage` для управления `null`.
- `FormattedValueValidation`: параметр `FormatErrorMessage` для ошибок формата.

## StringLength

Проверяет длину строки.

Параметры:
- `Length` — эталонная длина.
- `Equal` — разрешить равенство длины.
- `Less` — разрешить длину меньше эталонной.
- `Gatherer` — разрешить длину больше эталонной.
- `AllowNull` — разрешить `null`.
- `AllowNotString` — разрешить нестроковые значения.

Пример:

```xml
<TextBox>
  <TextBox.Text>
    <Binding Path="UserName" UpdateSourceTrigger="PropertyChanged">
      <Binding.ValidationRules>
        <vr:StringLength Length="8" Equal="True" />
      </Binding.ValidationRules>
    </Binding>
  </TextBox.Text>
</TextBox>
```

## RegExp

Проверяет строку на соответствие регулярному выражению.

Параметры:
- `Expression` — регулярное выражение.
- `AllowNotString` — разрешить нестроковые значения.
- `NotStringErrorMessage` — сообщение для нестроковых значений.
- `AllowNull` — разрешить `null`.
- `NullReferenceMessage` — сообщение для `null`.
- `FormatErrorMessage` — сообщение для несовпадения.
- `ErrorMessage` — сообщение по умолчанию.

Пример:

```xml
<TextBox>
  <TextBox.Text>
    <Binding Path="Email" UpdateSourceTrigger="PropertyChanged">
      <Binding.ValidationRules>
        <vr:RegExp Expression="^[^@\s]+@[^@\s]+\.[^@\s]+$" />
      </Binding.ValidationRules>
    </Binding>
  </TextBox.Text>
</TextBox>
```

## NotNull

Проверяет, что значение не `null`.

Параметры:
- `ErrorMessage` — сообщение об ошибке.

Пример:

```xml
<TextBox>
  <TextBox.Text>
    <Binding Path="Title" UpdateSourceTrigger="PropertyChanged">
      <Binding.ValidationRules>
        <vr:NotNull ErrorMessage="Значение обязательно" />
      </Binding.ValidationRules>
    </Binding>
  </TextBox.Text>
</TextBox>
```

## IsDouble

Проверяет, что значение может быть преобразовано в `double`.

Параметры:
- `AllowNull` — разрешить `null`.
- `NullReferenceMessage` — сообщение для `null`.
- `FormatErrorMessage` — сообщение для ошибок формата.
- `ErrorMessage` — сообщение по умолчанию.

Пример:

```xml
<TextBox>
  <TextBox.Text>
    <Binding Path="Price" UpdateSourceTrigger="PropertyChanged">
      <Binding.ValidationRules>
        <vr:IsDouble />
      </Binding.ValidationRules>
    </Binding>
  </TextBox.Text>
</TextBox>
```

## IsInteger

Проверяет, что значение может быть преобразовано в `int`.

Параметры:
- `AllowNull` — разрешить `null`.
- `NullReferenceMessage` — сообщение для `null`.
- `FormatErrorMessage` — сообщение для ошибок формата.
- `ErrorMessage` — сообщение по умолчанию.

Пример:

```xml
<TextBox>
  <TextBox.Text>
    <Binding Path="Count" UpdateSourceTrigger="PropertyChanged">
      <Binding.ValidationRules>
        <vr:IsInteger />
      </Binding.ValidationRules>
    </Binding>
  </TextBox.Text>
</TextBox>
```

## IsNumeric

Проверяет, что значение может быть преобразовано в `double` или `int`.

Параметры:
- `IntegerOnly` — проверять только целые числа.
- `AllowNull` — разрешить `null`.
- `NullReferenceMessage` — сообщение для `null`.
- `FormatErrorMessage` — сообщение для ошибок формата.
- `ErrorMessage` — сообщение по умолчанию.

Пример:

```xml
<TextBox>
  <TextBox.Text>
    <Binding Path="Amount" UpdateSourceTrigger="PropertyChanged">
      <Binding.ValidationRules>
        <vr:IsNumeric IntegerOnly="False" />
      </Binding.ValidationRules>
    </Binding>
  </TextBox.Text>
</TextBox>
```

## NotNaN

Проверяет, что значение — `double` и оно не `NaN`.

Параметры:
- `AllowNull` — разрешить `null`.
- `NullReferenceMessage` — сообщение для `null`.
- `FormatErrorMessage` — сообщение для ошибок формата.
- `ErrorMessage` — сообщение по умолчанию.

Пример:

```xml
<TextBox>
  <TextBox.Text>
    <Binding Path="Ratio" UpdateSourceTrigger="PropertyChanged">
      <Binding.ValidationRules>
        <vr:NotNaN />
      </Binding.ValidationRules>
    </Binding>
  </TextBox.Text>
</TextBox>
```

## ValueGreaterThan

Проверяет, что значение больше указанного.

Параметры:
- `Value` — эталонное значение.
- `IsEqual` — разрешить равенство.
- `ErrorMessage` — сообщение об ошибке.

Пример:

```xml
<TextBox>
  <TextBox.Text>
    <Binding Path="Speed" UpdateSourceTrigger="PropertyChanged">
      <Binding.ValidationRules>
        <vr:ValueGreaterThan Value="0" IsEqual="False" />
      </Binding.ValidationRules>
    </Binding>
  </TextBox.Text>
</TextBox>
```

## ValueLessThan

Проверяет, что значение меньше указанного.

Параметры:
- `Value` — эталонное значение.
- `IsEquals` — разрешить равенство.
- `ErrorMessage` — сообщение об ошибке.

Пример:

```xml
<TextBox>
  <TextBox.Text>
    <Binding Path="Discount" UpdateSourceTrigger="PropertyChanged">
      <Binding.ValidationRules>
        <vr:ValueLessThan Value="100" IsEquals="True" />
      </Binding.ValidationRules>
    </Binding>
  </TextBox.Text>
</TextBox>
```

## ValueInInterval

Проверяет, что значение находится внутри интервала.

Параметры:
- `Min` — минимальная граница.
- `Max` — максимальная граница.
- `MinEquals` — разрешить равенство `Min`.
- `MaxEquals` — разрешить равенство `Max`.
- `ErrorMessage` — сообщение об ошибке.

Пример:

```xml
<TextBox>
  <TextBox.Text>
    <Binding Path="Percentage" UpdateSourceTrigger="PropertyChanged">
      <Binding.ValidationRules>
        <vr:ValueInInterval Min="0" Max="100" MinEquals="True" MaxEquals="True" />
      </Binding.ValidationRules>
    </Binding>
  </TextBox.Text>
</TextBox>
```

## DirectoryExist

Проверяет существование директории.

Параметры:
- `AllowNull` — разрешить `null`.
- `NullReferenceMessage` — сообщение для `null`.
- `FormatErrorMessage` — сообщение для ошибок формата.
- `ErrorMessage` — сообщение по умолчанию.

Пример:

```xml
<TextBox>
  <TextBox.Text>
    <Binding Path="Folder" UpdateSourceTrigger="PropertyChanged">
      <Binding.ValidationRules>
        <vr:DirectoryExist />
      </Binding.ValidationRules>
    </Binding>
  </TextBox.Text>
</TextBox>
```

## FileExist

Проверяет существование файла.

Параметры:
- `AllowNull` — разрешить `null`.
- `NullReferenceMessage` — сообщение для `null`.
- `FormatErrorMessage` — сообщение для ошибок формата.
- `ErrorMessage` — сообщение по умолчанию.

Пример:

```xml
<TextBox>
  <TextBox.Text>
    <Binding Path="FilePath" UpdateSourceTrigger="PropertyChanged">
      <Binding.ValidationRules>
        <vr:FileExist />
      </Binding.ValidationRules>
    </Binding>
  </TextBox.Text>
</TextBox>
```

## PathExist

Проверяет существование пути в файловой системе.

Параметры:
- `AllowNull` — разрешить `null`.
- `ErrorMessage` — сообщение об ошибке.

Пример:

```xml
<TextBox>
  <TextBox.Text>
    <Binding Path="AnyPath" UpdateSourceTrigger="PropertyChanged">
      <Binding.ValidationRules>
        <vr:PathExist AllowNull="True" />
      </Binding.ValidationRules>
    </Binding>
  </TextBox.Text>
</TextBox>
```

## Подключение пространства имён

Примеры выше используют префикс `vr`, подключаемый так:

```xml
xmlns:vr="clr-namespace:MathCore.WPF.ValidationRules"
```
