using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace MathCore.WPF.TeX;

/// <summary>
/// Визуальный бокс - графический элемент математического выражения, который может содержать дочерние боксы
/// </summary>
/// <remarks>
/// Боксы - это результат преобразования атомов (Atom) в визуальное представление.
/// Каждый бокс содержит:
/// - Размеры (ширину, высоту, глубину)
/// - Визуальные свойства (кисти для переднего плана и фона)
/// - Дочерние боксы (для составных элементов)
/// 
/// Различные типы боксов отрисовывают себя по-разному:
/// CharBox отрисовывает символ, HorizontalBox - ряд боксов, FractionBox - дробь и т.д.
/// </remarks>
public abstract class Box
{
    private readonly List<Box> _Children;
    private readonly ReadOnlyCollection<Box> _ChildrenReadOnly;

    /// <summary>Дочерние боксы, входящие в состав этого бокса</summary>
    public ReadOnlyCollection<Box> Children => _ChildrenReadOnly;

    /// <summary>Кисть для отрисовки переднего плана (символы, линии и т.д.)</summary>
    public Brush? Foreground { get; set; }

    /// <summary>Кисть для отрисовки фонового прямоугольника</summary>
    public Brush Background { get; set; }

    /// <summary>
    /// Общая высота бокса (высота над базовой линией + глубина ниже базовой линии)
    /// </summary>
    public double TotalHeight => Height + Depth;

    /// <summary>Ширина бокса (в единицах относительного размера, масштабируется при рендеринге)</summary>
    public double Width { get; set; }

    /// <summary>Высота бокса выше базовой линии (в единицах относительного размера)</summary>
    public double Height { get; set; }

    /// <summary>Глубина бокса ниже базовой линии (в единицах относительного размера)</summary>
    public double Depth { get; set; }

    /// <summary>Вертикальное смещение бокса от базовой линии (положительное = вверх)</summary>
    public double Shift { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр бокса с использованием визуальных свойств из среды
    /// </summary>
    /// <param name="environment">Среда рендеринга с информацией о цветах и стилях</param>
    internal Box(TexEnvironment environment) : this(environment.Foreground, environment.Background) { }

    /// <summary>Инициализирует новый экземпляр пустого бокса</summary>
    protected Box() : this(null, null) { }

    /// <summary>
    /// Инициализирует новый экземпляр бокса с заданными визуальными свойствами
    /// </summary>
    /// <param name="foreground">Кисть для переднего плана</param>
    /// <param name="background">Кисть для фона</param>
    protected Box(Brush foreground, Brush background)
    {
        _Children         = [];
        _ChildrenReadOnly = new(_Children);
        Foreground       = foreground;
        Background       = background;
    }

    /// <summary>
    /// Отрисовывает бокс и его дочерние элементы в WPF DrawingContext
    /// </summary>
    /// <param name="Context">WPF контекст для отрисовки</param>
    /// <param name="scale">Коэффициент масштабирования (обычно размер шрифта)</param>
    /// <param name="x">Координата X в пиксельных точках</param>
    /// <param name="y">Координата Y в пиксельных точках (базовая линия)</param>
    /// <remarks>
    /// Базовая реализация отрисовывает фоновый прямоугольник, если задана кисть для фона.
    /// Подклассы переопределяют этот метод для отрисовки своего специфичного содержимого.
    /// </remarks>
    public virtual void Draw(DrawingContext Context, double scale, double x, double y)
    {
        if(Background is null) return;
        // Отрисовать фон бокса прямоугольником
        Context.DrawRectangle(Background, null, new(x * scale, (y - Height) * scale,
            Width * scale, (Height + Depth) * scale));
    }

    /// <summary>Добавляет дочерний бокс в конец списка дочерних элементов</summary>
    /// <param name="box">Бокс для добавления</param>
    public virtual void Add(Box box) => _Children.Add(box);

    /// <summary>
    /// Вставляет дочерний бокс в указанную позицию в списке дочерних элементов
    /// </summary>
    /// <param name="position">Позиция для вставки (0-основана)</param>
    /// <param name="box">Бокс для вставки</param>
    public virtual void Add(int position, Box box) => _Children.Insert(position, box);

    /// <summary>
    /// Получает идентификатор последнего используемого шрифта в этом боксе и его дочерних элементах
    /// </summary>
    /// <returns>Идентификатор шрифта</returns>
    /// <remarks>
    /// Используется для оптимизации - когда несколько элементов используют один шрифт,
    /// можно избежать переключения контекста рендеринга.
    /// </remarks>
    public abstract int GetLastFontId();
}