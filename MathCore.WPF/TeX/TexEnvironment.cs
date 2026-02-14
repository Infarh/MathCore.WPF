using System.Windows.Media;

namespace MathCore.WPF.TeX;

/// <summary>
/// Среда рендеринга, определяющая текущие графические параметры для создания боксов
/// </summary>
/// <remarks>
/// Класс служит контейнером для всей информации, необходимой при преобразовании атомов в боксы:
/// - Математический стиль (Display, Text, Script, ScriptScript)
/// - Используемый TeX шрифт с метриками
/// - Цвета переднего плана и фона
/// - ID последнего используемого шрифта для оптимизации
/// 
/// Среда выполняет две основные функции:
/// 1. Хранит контекстную информацию, передаваемую вниз по иерархии атомов
/// 2. Предоставляет методы для создания новых сред с модифицированными стилями
/// </remarks>
internal class TexEnvironment
{
    /// <summary>ID шрифта, который был использован последним</summary>
    private int _LastFontId = TexFontUtilities.NoFontId;

    /// <summary>Текущий математический стиль для рендеринга</summary>
    public TexStyle Style { get; private set; }

    /// <summary>TeX шрифт, используемый для получения метрик и информации о символах</summary>
    public ITeXFont TexFont { get; }

    /// <summary>Кисть для отрисовки фонового прямоугольника</summary>
    public Brush Background { get; set; }

    /// <summary>Кисть для отрисовки переднего плана (символы, линии и т.д.)</summary>
    public Brush Foreground { get; set; }

    /// <summary>
    /// ID последнего используемого шрифта (используется для оптимизации при рендеринге)
    /// </summary>
    public int LastFontId
    {
        get => _LastFontId == TexFontUtilities.NoFontId ? TexFont.GetMuFontId() : _LastFontId;
        set => _LastFontId = value;
    }

    /// <summary>
    /// Инициализирует новую среду рендеринга с заданным стилем и шрифтом
    /// </summary>
    /// <param name="style">Математический стиль</param>
    /// <param name="TexFont">TeX шрифт с метриками</param>
    public TexEnvironment(TexStyle style, ITeXFont TexFont) : this(style, TexFont, null, null) { }

    /// <summary>
    /// Инициализирует новую среду с полным набором параметров
    /// </summary>
    /// <param name="style">Математический стиль</param>
    /// <param name="TexFont">TeX шрифт</param>
    /// <param name="background">Кисть для фона (может быть null)</param>
    /// <param name="foreground">Кисть для переднего плана (может быть null)</param>
    private TexEnvironment(TexStyle style, ITeXFont TexFont, Brush background, Brush foreground)
    {
        // Валидировать и установить стиль
        if(style is TexStyle.Display or TexStyle.Text or TexStyle.Script or TexStyle.ScriptScript)
            Style = style;
        else
            Style = TexStyle.Display;

        this.TexFont    = TexFont;
        Background = background;
        Foreground = foreground;
    }

    /// <summary>
    /// Получает сжатый стиль для специальных типов атомов
    /// </summary>
    /// <returns>Новая среда со сжатым стилем</returns>
    /// <remarks>
    /// Сжатый стиль используется для некоторых специальных атомов
    /// (например, для радикалов и индексов).
    /// </remarks>
    public TexEnvironment GetCrampedStyle()
    {
        var new_environment = Clone();
        new_environment.Style = (int)Style % 2 == 1 ? Style : Style + 1;
        return new_environment;
    }

    /// <summary>
    /// Получает стиль для числителя дроби
    /// </summary>
    /// <returns>Новая среда со стилем для числителя</returns>
    /// <remarks>
    /// Стиль для числителя обычно меньше, чем для основного выражения,
    /// так как дроби с дробями должны выглядеть меньше.
    /// </remarks>
    public TexEnvironment GetNumeratorStyle()
    {
        var new_environment = Clone();
        new_environment.Style = Style + 2 - 2 * ((int)Style / 6);
        return new_environment;
    }

    /// <summary>
    /// Получает стиль для знаменателя дроби
    /// </summary>
    /// <returns>Новая среда со стилем для знаменателя</returns>
    /// <remarks>
    /// Стиль для знаменателя обычно меньше, чем для основного выражения.
    /// </remarks>
    public TexEnvironment GetDenominatorStyle()
    {
        var new_environment = Clone();
        new_environment.Style = (TexStyle)(2 * ((int)Style / 2) + 1 + 2 - 2 * ((int)Style / 6));
        return new_environment;
    }

    /// <summary>
    /// Получает стиль для индекса (степень корня)
    /// </summary>
    /// <returns>Новая среда со стилем ScriptScript</returns>
    public TexEnvironment GetRootStyle()
    {
        var new_environment = Clone();
        new_environment.Style = TexStyle.ScriptScript;
        return new_environment;
    }

    /// <summary>
    /// Получает стиль для подстрочного индекса
    /// </summary>
    /// <returns>Новая среда со стилем для подстрочного индекса</returns>
    /// <remarks>
    /// Подстрочный индекс обычно меньше основного выражения.
    /// </remarks>
    public TexEnvironment GetSubscriptStyle()
    {
        var new_environment = Clone();
        new_environment.Style = (TexStyle)(2 * ((int)Style / 4) + 4 + 1);
        return new_environment;
    }

    /// <summary>
    /// Получает стиль для надстрочного индекса
    /// </summary>
    /// <returns>Новая среда со стилем для надстрочного индекса</returns>
    /// <remarks>
    /// Надстрочный индекс обычно меньше основного выражения.
    /// </remarks>
    public TexEnvironment GetSuperscriptStyle()
    {
        var new_environment = Clone();
        new_environment.Style = (TexStyle)(2 * ((int)Style / 4) + 4 + ((int)Style % 2));
        return new_environment;
    }

    /// <summary>
    /// Создаёт копию текущей среды с сохранением всех параметров
    /// </summary>
    /// <returns>Новая среда - клон текущей</returns>
    public TexEnvironment Clone() => new(Style, TexFont, Background, Foreground);

    /// <summary>Очищает цвета переднего плана и фона, но сохраняет стиль и шрифт</summary>
    public void Reset()
    {
        Background = null;
        Foreground = null;
    }
}