namespace MathCore.WPF.TeX;

/// <summary>
/// Атом (наименьшая единица) TeX формулы
/// </summary>
/// <remarks>
/// Атомы - это строительные блоки всех математических формул в TeX модуле.
/// Каждый атом отвечает за:
/// 1. Хранение своих данных (символы, подформулы и т.д.)
/// 2. Определение своего типа (Ordinary, BinaryOperator, Relation и т.д.)
/// 3. Преобразование себя в визуальный бокс для рендеринга
/// 
/// Существуют различные типы атомов: CharAtom (символ), RowAtom (ряд атомов),
/// FractionAtom (дробь), ScriptsAtom (индексы) и другие.
/// </remarks>
internal abstract class Atom
{
    /// <summary>Инициализирует новый экземпляр атома со стандартным типом Ordinary</summary>
    protected Atom() => Type = TexAtomType.Ordinary;

    /// <summary>
    /// Тип атома, определяющий его математические свойства и расстояния до соседних элементов
    /// </summary>
    public TexAtomType Type { get; set; }

    /// <summary>
    /// Преобразует атом в визуальный бокс для рендеринга в указанной среде
    /// </summary>
    /// <param name="environment">Среда рендеринга с информацией о стиле, шрифте и других параметрах</param>
    /// <returns>Бокс, который содержит визуальное представление этого атома</returns>
    /// <remarks>
    /// Каждый подкласс Atom реализует этот метод для преобразования своих данных
    /// в визуальное представление. Рекурсивно вызывается для всех дочерних атомов.
    /// </remarks>
    public abstract Box CreateBox(TexEnvironment environment);

    /// <summary>
    /// Получает тип самого левого дочернего элемента (для подсчёта расстояний)
    /// </summary>
    /// <returns>Тип TexAtomType самого левого элемента</returns>
    /// <remarks>
    /// Используется при подсчёте расстояний между соседними атомами.
    /// Для составных атомов (RowAtom, FractionAtom) возвращает тип левого потомка.
    /// </remarks>
    public virtual TexAtomType GetLeftType() => Type;

    /// <summary>
    /// Получает тип самого правого дочернего элемента (для подсчёта расстояний)
    /// </summary>
    /// <returns>Тип TexAtomType самого правого элемента</returns>
    /// <remarks>
    /// Используется при подсчёте расстояний между соседними атомами.
    /// Для составных атомов (RowAtom, FractionAtom) возвращает тип правого потомка.
    /// </remarks>
    public virtual TexAtomType GetRightType() => Type;
}